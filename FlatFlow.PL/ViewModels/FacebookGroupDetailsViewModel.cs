using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.PL.ViewModels
{
    public class FacebookGroupDetailsViewModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string? GroupLink { get; set; }
        public int PostsCount { get; set; }
        public List<ApartmentGroupPost>? ApartmentPosts { get; set; }


    }
}
