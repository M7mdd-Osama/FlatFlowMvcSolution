using FlatFlow.DAL.Models.Identity;
using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Models
{
    public class FacebookGroup : BaseEntity
    {
        public string GroupName { get; set; }
        public string GroupLink { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<ApartmentGroupPost> ApartmentGroupPosts { get; set; }
    }
}
