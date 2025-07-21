namespace FlatFlow.PL.ViewModels
{
    public class ApartmentCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsRented { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsVideo { get; set; }
    }
}