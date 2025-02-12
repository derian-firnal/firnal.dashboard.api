namespace firnal.dashboard.services.Interfaces
{
    public interface ISchemaService
    {
        Task<List<string>> GetAll();
    }
}
