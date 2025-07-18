using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Data.Configurations
{
    public class FacebookGroupConfig : BaseEntityConfig<FacebookGroup>, IEntityTypeConfiguration<FacebookGroup>
    {
        public new void Configure(EntityTypeBuilder<FacebookGroup> builder)
        {
            builder.Property(g => g.GroupName).IsRequired().HasMaxLength(200);
            builder.Property(g => g.GroupLink).IsRequired().HasMaxLength(500);
        }
    }
}
