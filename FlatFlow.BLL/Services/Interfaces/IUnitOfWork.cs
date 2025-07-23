using FlatFlow.DAL.Repositories.Interfaces;

namespace Demo.BLL.Interfaces
{
    public interface IUnitOfWork
    {
        public IApartmentGroupPostRepo ApartmentGroupPostRepo { get; set; }
        public IApartmentRepo ApartmentRepo { get; set; }
        public IClientRepo ClientRepo { get; set; }
        Task<int> CompleteAsync();
    }
}