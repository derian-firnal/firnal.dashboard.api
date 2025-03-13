using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;

namespace firnal.dashboard.repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;
        private const string DbName = "OUTREACHGENIUS_DRIPS";
        private const string SchemaName = "PUBLIC";

        public AuthRepository(SnowflakeDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            using var conn = _dbFactory.GetConnection();
            
            // SQL query using a LEFT JOIN to include any associated schemas.
            string query = $@"
                SELECT 
                    u.Id, 
                    u.UserName, 
                    u.Email, 
                    u.PasswordHash,
                    us.UserId as SchemaUserId, 
                    us.SchemaName
                FROM {DbName}.{SchemaName}.Users u
                LEFT JOIN {DbName}.{SchemaName}.UserSchemas us ON u.Id = us.UserId
                WHERE u.Email = :Email";

            // Use a dictionary to group User records and their associated UserSchema entries
            var userDictionary = new Dictionary<string, User>();

            var user = await conn.QueryAsync<User, UserSchema, User>(
                query,
                (user, userSchema) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var currentUser))
                    {
                        currentUser = user;
                        currentUser.Schemas = new List<UserSchema>();
                        userDictionary.Add(user.Id, currentUser);
                    }

                    // Only add schema if it exists (it can be null if no mapping is found)
                    // Using the alias "SchemaUserId" in the query to split the mapping
                    if (userSchema != null && !string.IsNullOrEmpty(userSchema.SchemaName))
                    {
                        currentUser?.Schemas?.Add(userSchema);
                    }
                    return currentUser;
                },
                new { Email = email },
                splitOn: "SchemaUserId" // Tells Dapper where to split the result between User and UserSchema objects.
            );

            var userResult = userDictionary.Values.FirstOrDefault();

            if (BCrypt.Net.BCrypt.Verify(password, userResult?.PasswordHash))
            {
                return userResult;
            }

            return null;
        }

        public async Task<string?> RegisterUser(string email, string username, string password, string role, List<string>? schemas)
        {
            using var conn = _dbFactory.GetConnection();
            
            using var transaction = conn.BeginTransaction();

            try
            {
                string userId = Guid.NewGuid().ToString();
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Step 1: Insert User using colon-prefixed parameters
                string userInsertQuery = $@"INSERT INTO {DbName}.{SchemaName}.Users (ID, USERNAME, EMAIL, PASSWORDHASH) VALUES (:ID, :USERNAME, :EMAIL, :PASSWORDHASH)";

                int userResult = await conn.ExecuteAsync(userInsertQuery, new
                {
                    ID = userId,
                    USERNAME = username,
                    EMAIL = email,
                    PASSWORDHASH = passwordHash
                }, transaction);

                if (userResult <= 0) throw new Exception("Failed to insert user.");

                // Step 2: Lookup Role ID using colon-prefixed parameter
                string roleQuery = $@"SELECT ID FROM {DbName}.{SchemaName}.ROLES WHERE NAME = :Name";

                var roleId = await conn.ExecuteScalarAsync<string>(roleQuery, new
                {
                    Name = role
                }, transaction);

                if (string.IsNullOrEmpty(roleId)) throw new Exception("Role not found.");

                // Step 3: Insert User Role using colon-prefixed parameters
                string roleInsertQuery = $@"INSERT INTO {DbName}.{SchemaName}.UserRoles (UserId, RoleId) VALUES (:UserId, :RoleId)";

                int roleResult = await conn.ExecuteAsync(roleInsertQuery, new
                {
                    UserId = userId,
                    RoleId = roleId
                }, transaction);

                if (roleResult <= 0) throw new Exception("Failed to insert user role.");

                // Step 4: Insert User Schema Mappings using colon-prefixed parameters
                string schemaInsertQuery = $@"INSERT INTO {DbName}.{SchemaName}.UserSchemas (UserId, SchemaName) VALUES (:UserId, :SchemaName)";

                if (schemas != null && schemas.Count > 0)
                {
                    foreach (var schema in schemas)
                    {
                        int schemaResult = await conn.ExecuteAsync(schemaInsertQuery, new
                        {
                            UserId = userId,
                            SchemaName = schema
                        }, transaction);

                        if (schemaResult <= 0) throw new Exception($"Failed to insert schema access for {schema}.");
                    }
                }

                // Commit transaction on success
                transaction.Commit();
                return userId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in RegisterUser: {ex.Message}");
                return null;
            }
        }




    }
}
