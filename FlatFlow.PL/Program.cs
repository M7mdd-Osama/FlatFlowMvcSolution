using FlatFlow.BLL.DTOs;
using FlatFlow.DAL.Data.DbContexts;
using FlatFlow.DAL.Models;
using FlatFlow.DAL.Models.ApartmentModel;
using FlatFlow.DAL.Models.Identity;
using FlatFlow.DAL.Repositories.Classes;
using FlatFlow.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmailSender = FlatFlow.BLL.Services.Classes.EmailSender;
using IEmailSender = FlatFlow.BLL.Services.Interfaces.IEmailSender;

namespace FlatFlow.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.

            builder.Services.AddControllersWithViews();

            // Database Context Configuration
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Repository Registrations
            builder.Services.AddScoped<IGenericRepo<Client>, GenericRepo<Client>>();
            builder.Services.AddScoped<IGenericRepo<Apartment>, GenericRepo<Apartment>>();
            builder.Services.AddScoped<IApartmentRepo, ApartmentRepo>();
            builder.Services.AddScoped<IClientRepo, ClientRepo>();

            // Authentication Configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

            // Identity Configuration
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Tokens.ProviderMap.Add("Default",
                    new TokenProviderDescriptor(typeof(IUserTwoFactorTokenProvider<User>)));
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Cookie Configuration
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Security Stamp Configuration
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(30);
            });

            // Email Services Configuration
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB
            });
            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            #endregion

            app.Run();
        }
    }
}