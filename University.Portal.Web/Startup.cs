using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using University.Portal.Data.Data;
using University.Portal.Data.Interface;
using University.Portal.Web.Extensions;

namespace University.Portal.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AppDBContext>(options =>
				options.UseSqlServer(
				Configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly(typeof(AppDBContext).Assembly.FullName)));

			services.AddMemoryCache();			
			services.AddApplicationServices();			

			services.AddControllersWithViews().AddRazorRuntimeCompilation();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
					options.SlidingExpiration = true;
					options.AccessDeniedPath = "/Forbidden/";
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// Configure the HTTP request pipeline.
			if (!env.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			else
			{
                app.UseMiddleware<ExceptionMiddleware>();
            }

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();			

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
				   name: "default",
				   pattern: "{controller=Dashboard}/{action=Index}/{id?}");
			});
		}
	}
}
