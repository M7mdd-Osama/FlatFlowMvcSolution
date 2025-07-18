using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Pass).IsRequired().HasMaxLength(255);
        }
    }
}
