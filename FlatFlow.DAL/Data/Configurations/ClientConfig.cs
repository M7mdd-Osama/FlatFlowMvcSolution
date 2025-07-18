namespace FlatFlow.DAL.Data.Configurations
{
    public class ClientConfig : BaseEntityConfig<Client>, IEntityTypeConfiguration<Client>
    {
        public new void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Phone).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Status).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Note).HasMaxLength(1000);
            builder.Property(c => c.Commission).HasColumnType("decimal(18,2)");

            builder.HasOne(c => c.Apartment)
                   .WithMany(a => a.Clients)
                   .HasForeignKey(c => c.ApartmentId);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Clients)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
