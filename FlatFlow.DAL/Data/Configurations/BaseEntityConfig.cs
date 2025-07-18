using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Data.Configurations
{
    public class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedBy).IsRequired();

            builder.Property(e => e.LastModifiedBy).IsRequired();

            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(e => e.CreatedOn)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.LastModifiedOn)
                   .HasComputedColumnSql("GETDATE()");
        }
    }
}
