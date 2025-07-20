namespace FlatFlow.PL.ViewModels
{
    public class ApartmentIndexViewModel
    {
        public List<ApartmentCardViewModel> Apartments { get; set; } = new List<ApartmentCardViewModel>();
        public int TotalApartments { get; set; }
        public int AvailableCount { get; set; }
        public int RentedCount { get; set; }
        public decimal TotalCommission { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsRentedFilter { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
