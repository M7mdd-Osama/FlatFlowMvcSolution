namespace FlatFlow.PL.ViewModels
{
    public class ApartmentCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public bool IsRented { get; set; }
        public string? ImageUrl { get; set; }
    }
}
