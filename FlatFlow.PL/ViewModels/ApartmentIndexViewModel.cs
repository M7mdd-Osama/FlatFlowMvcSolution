namespace FlatFlow.PL.ViewModels
{
    public class ApartmentIndexViewModel
    {
        public List<ApartmentCardViewModel> Apartments { get; set; } = new List<ApartmentCardViewModel>();

        // Pagination Properties
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // Statistics
        public int TotalApartments { get; set; }
        public int AvailableCount { get; set; }
        public int RentedCount { get; set; }

        // Search Filters
        public string? SearchTerm { get; set; }
        public bool? IsRentedFilter { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Helper Methods for Pagination
        public int StartItem => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
    }
}
