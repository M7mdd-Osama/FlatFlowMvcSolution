namespace FlatFlow.DAL.Repositories.Interfaces
{
    public interface IClientRepo : IGenericRepo<Client>
    {
        IEnumerable<Client> GetClientsByUserId(string userId);
    }
}
