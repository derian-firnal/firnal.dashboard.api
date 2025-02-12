using firnal.dashboard.repositories.Interfaces;
using firnal.dashboard.services.Interfaces;

namespace firnal.dashboard.services
{
    public class SchemaService : ISchemaService
    {
        private readonly ISchemaRepository _schemaRepository;

        public SchemaService(ISchemaRepository schemaRepository)
        {
            _schemaRepository = schemaRepository;
        }

        public async Task<List<string>> GetAll()
        {
            return await _schemaRepository.GetAll();
        }
    }
}
