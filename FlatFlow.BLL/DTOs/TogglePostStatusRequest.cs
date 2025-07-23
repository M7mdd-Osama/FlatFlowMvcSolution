namespace FlatFlow.BLL.DTOs
{
    public class TogglePostStatusRequest
    {
        public int ApartmentId { get; set; }
        public int GroupId { get; set; }
        public bool IsPosted { get; set; }
    }
}
