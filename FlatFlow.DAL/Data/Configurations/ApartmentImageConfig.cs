using FlatFlow.DAL.Models.ApartmentModel;

namespace FlatFlow.DAL.Data.Configurations
{
    public class ApartmentImageConfig : BaseEntityConfig<ApartmentImage>, IEntityTypeConfiguration<ApartmentImage>
    {
        public new void Configure(EntityTypeBuilder<ApartmentImage> builder)
        {
            builder.Property(p => p.Url).IsRequired().HasMaxLength(500);

            builder.HasOne(p => p.Apartment)
                   .WithMany(a => a.ApartmentImages)
                   .HasForeignKey(p => p.ApartmentId);
        }
    }
}
