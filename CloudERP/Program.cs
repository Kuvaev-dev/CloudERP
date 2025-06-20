using CloudERP.Helpers;
using Domain.ServiceAccess;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Utils.Helpers;

namespace CloudERP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient<IHttpClientHelper, HttpClientHelper>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<ISessionHelper, SessionHelper>();
            builder.Services.AddScoped<IImageUploadHelper, ImageUploadHelper>();
            builder.Services.AddScoped<IPhoneNumberHelper, PhoneNumberHelper>();
            builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();

            builder.Services.AddScoped<CurrencyRatesFilter>();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CurrencyRatesFilter>();
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login";
                    options.LogoutPath = "/Home/Logout";
                    options.AccessDeniedPath = "/EP/EP500";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var defaultCulture = new CultureInfo("en-US");
            var supportedCultures = new List<CultureInfo>
            {
                new("en-US"),
                new("uk-UA")
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseRequestLocalization(localizationOptions);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}"
            );
            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
