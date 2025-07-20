namespace FlatFlow.PL.ViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Commission { get; set; }
    }
}
