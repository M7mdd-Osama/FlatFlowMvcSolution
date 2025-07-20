namespace FlatFlow.PL.ViewModels
{
    public class ApartmentDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRented { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<ClientViewModel> Clients { get; set; } = new List<ClientViewModel>();
    }
}
