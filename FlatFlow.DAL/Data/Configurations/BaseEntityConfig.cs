using FlatFlow.DAL.Models.Shared;

namespace FlatFlow.DAL.Data.Configurations
{
    public class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

        }
    }
}
