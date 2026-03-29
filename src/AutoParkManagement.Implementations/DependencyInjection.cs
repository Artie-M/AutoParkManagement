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
            x.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        });
    }
}