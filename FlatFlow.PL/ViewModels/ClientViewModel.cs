namespace FlatFlow.PL.ViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public decimal? Commission { get; set; }
        public string Note { get; set; }

        // Navigation property for apartment
        public ApartmentViewModel Apartment { get; set; }
    }
}
