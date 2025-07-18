using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Repositories.Classes;
using FlatFlow.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IGenericRepo<Client>, GenericRepo<Client>>();
            builder.Services.AddScoped<IGenericRepo<Apartment>, GenericRepo<Apartment>>();
            builder.Services.AddScoped<IApartmentRepo, ApartmentRepo>();

            builder.Services.AddHttpContextAccessor();
            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Apartment}/{action=Index}/{id?}");

            #endregion

            app.Run();
        }
    }
}
