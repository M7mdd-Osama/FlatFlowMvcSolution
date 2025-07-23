using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Repositories.Interfaces
{
    public interface IApartmentGroupPostRepo
    {
        Task<List<ApartmentGroupPost>> GetByApartmentIdAsync(int apartmentId);
        Task TogglePostStatusAsync(int apartmentId, int groupId, bool isPosted);
        Task<bool> IsPostedAsync(int apartmentId, int groupId);

    }
}
