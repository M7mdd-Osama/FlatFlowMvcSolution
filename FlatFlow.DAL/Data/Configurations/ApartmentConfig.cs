using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Data.Configurations
{
    public class ApartmentConfig : BaseEntityConfig<Apartment>, IEntityTypeConfiguration<Apartment>
    {
        public new void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(a => a.Location).HasMaxLength(200);
            builder.Property(a => a.Description).HasMaxLength(1000);

            builder.HasOne(a => a.User)
                   .WithMany(u => u.Apartments)
                   .HasForeignKey(a => a.UserId);
        }
    }
}
