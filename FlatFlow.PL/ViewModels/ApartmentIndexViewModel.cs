namespace FlatFlow.PL.ViewModels
{
    public class ApartmentIndexViewModel
    {
        public List<ApartmentCardViewModel> Apartments { get; set; }
        public int TotalApartments { get; set; }
        public decimal TotalCommission { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsRentedFilter { get; set; }
    }
}
