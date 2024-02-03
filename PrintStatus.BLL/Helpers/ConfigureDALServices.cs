using Microsoft.Extensions.DependencyInjection;
using PrintStatus.DAL.Connection;

namespace PrintStatus.BLL.Helpers
{
	public static class BLLRegistration
	{
		public static void ConfigureDALServices(this IServiceCollection services, string connectionString)
		{
			DALRegistration.ConfigureDAL(services, connectionString);
		}
	}
}
