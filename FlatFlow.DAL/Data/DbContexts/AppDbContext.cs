using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Models.Identity;
using FlatFlow.DAL.Models.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace FlatFlow.DAL.Data.DbContexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentImage> ApartmentImages { get; set; }
        public DbSet<ApartmentGroupPost> ApartmentGroupPosts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<FacebookGroup> FacebookGroups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}