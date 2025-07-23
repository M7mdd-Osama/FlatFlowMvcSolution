using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Data.Configurations
{
    public class ApartmentGroupPostConfig : BaseEntityConfig<ApartmentGroupPost>, IEntityTypeConfiguration<ApartmentGroupPost>
    {
        public new void Configure(EntityTypeBuilder<ApartmentGroupPost> builder)
        {
            builder.HasOne(p => p.Apartment)
                           .WithMany(a => a.ApartmentGroupPosts)
                           .HasForeignKey(p => p.ApartmentId);

            builder.HasOne(p => p.Group)
                   .WithMany(g => g.ApartmentGroupPosts)
                   .HasForeignKey(p => p.GroupId);
        }
    }
}
