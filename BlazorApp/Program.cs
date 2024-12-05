using BlazorApp.Components;
using BlazorApp.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Initializing the DBService.
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new DBService(connectionString);
            });

            // Create authentication.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "auth_token";
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.LoginPath = "/login";
            });

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseAntiforgery();

			app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

			// Maps the API controller for authentication.
			app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}
