namespace FlatFlow.PL.ViewModels
{
    public class ApartmentDetailsWithGroupsViewModel : ApartmentDetailsViewModel
    {
        public List<ApartmentGroupPostViewModel> FacebookGroups { get; set; } = new List<ApartmentGroupPostViewModel>();
    }
}
