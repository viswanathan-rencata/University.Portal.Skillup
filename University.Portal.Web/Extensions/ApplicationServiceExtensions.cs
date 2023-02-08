using University.Portal.Data.Data;
using University.Portal.Data.Interface;

namespace University.Portal.Web.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddTransient<IRepository<AppUser>, Repository<AppUser>>();
			services.AddTransient<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
