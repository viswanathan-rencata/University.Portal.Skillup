using University.Portal.Data.Data;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Interface;

namespace University.Portal.Web.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{			
            services.AddTransient<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}
