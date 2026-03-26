using AutoParkManagement.Common.Database;
using AutoParkManagement.Implementations.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoParkManagement.Implementations;

public static class DependencyInjection
{
    public static IServiceCollection AddDataStore(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddDbContext<ICoreContext, CoreContext>(x =>
        {
            x.UseMySql("server=localhost;port=3306;database=AutoPark;uid=root;pwd=root;", 
                serverVersion: new MySqlServerVersion(new Version(8, 4, 4)));
            //x.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        });
    }
}