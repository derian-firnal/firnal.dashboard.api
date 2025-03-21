﻿using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;

namespace firnal.dashboard.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;
        private const string DbName = "OUTREACHGENIUS_DRIPS";
        private const string SchemaName = "PUBLIC";

        public UserRepository(SnowflakeDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<User?> GetUserByEmail(string? email)
        {
            using var conn = _dbFactory.GetConnection();

            string query = $@"
                SELECT 
                    u.Id, 
                    u.UserName, 
                    u.Email, 
                    u.PasswordHash,
                    us.UserId AS SchemaUserId, 
                    us.SchemaName,
                    r.Id,
                    r.Name
                FROM {DbName}.{SchemaName}.Users u
                LEFT JOIN {DbName}.{SchemaName}.UserSchemas us ON u.Id = us.UserId
                LEFT JOIN {DbName}.{SchemaName}.UserRoles ur ON u.Id = ur.UserId
                LEFT JOIN {DbName}.{SchemaName}.Roles r ON ur.RoleId = r.Id
                WHERE u.Email = :Email";

            // Dictionary to group user rows (when a user has multiple schemas)
            var userDictionary = new Dictionary<string, User>();

            var result = await conn.QueryAsync<User, UserSchema, Role, User>(
                query,
                (user, userSchema, role) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var currentUser))
                    {
                        currentUser = user;
                        currentUser.Schemas = new List<UserSchema>();
                        // Assign role name from the Role object if available.
                        if (role != null)
                        {
                            currentUser.RoleName = role.Name;
                        }
                        userDictionary.Add(user.Id, currentUser);
                    }

                    // Add the schema if it exists.
                    if (userSchema != null && !string.IsNullOrEmpty(userSchema.SchemaName))
                    {
                        currentUser?.Schemas?.Add(userSchema);
                    }

                    return currentUser;
                },
                new { Email = email },
                splitOn: "SchemaUserId,Id" // Splits: UserSchema starts at SchemaUserId; Role starts at Id.
            );

            var userResult = userDictionary.Values.FirstOrDefault();

            return userResult;
        }

        public async Task<List<User>> GetAllUsers()
        {
            using var conn = _dbFactory.GetConnection();

            string query = $@"
                            SELECT 
                                u.Id, 
                                u.UserName, 
                                u.Email, 
                                u.PasswordHash,
                                us.UserId AS SchemaUserId, 
                                us.SchemaName,
                                r.Id,
                                r.Name
                            FROM {DbName}.{SchemaName}.Users u
                            LEFT JOIN {DbName}.{SchemaName}.UserSchemas us ON u.Id = us.UserId
                            LEFT JOIN {DbName}.{SchemaName}.UserRoles ur ON u.Id = ur.UserId
                            LEFT JOIN {DbName}.{SchemaName}.Roles r ON ur.RoleId = r.Id";

            // Use a dictionary keyed by email to ensure each email appears only once
            var userDictionary = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

            var result = await conn.QueryAsync<User, UserSchema, Role, User>(query, (user, userSchema, role) =>
            {
                // Use email as the unique key
                if (!userDictionary.TryGetValue(user.Email, out var currentUser))
                {
                    currentUser = user;
                    currentUser.Schemas = new List<UserSchema>();
                    if (role != null)
                    {
                        currentUser.RoleName = role.Name;
                    }
                    userDictionary.Add(user.Email, currentUser);
                }

                // If a schema is present and not already added, add it to the user's list
                if (userSchema != null && !string.IsNullOrEmpty(userSchema.SchemaName))
                {
                    if (!currentUser.Schemas.Any(s => s.SchemaName == userSchema.SchemaName))
                    {
                        currentUser.Schemas.Add(userSchema);
                    }
                }

                return currentUser;
            }, splitOn: "SchemaUserId,Id");

            return userDictionary.Values.ToList();
        }

    }
}
