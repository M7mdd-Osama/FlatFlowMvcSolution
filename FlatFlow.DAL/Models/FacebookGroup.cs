namespace FlatFlow.DAL.Models
{
    public class FacebookGroup : BaseEntity
    {
        public string GroupName { get; set; }
        public string GroupLink { get; set; }

        public ICollection<ApartmentGroupPost> ApartmentGroupPosts { get; set; }
    }
}
