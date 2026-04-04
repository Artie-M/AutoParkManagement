using AutoParkManagement.Common.Auth;
using AutoParkManagement.Common.Database;
using AutoParkManagement.Implementations.Auth;
using AutoParkManagement.Implementations.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AutoParkManagement.Implementations;

public static class DependencyInjection
{
    public static IServiceCollection AddDataStore(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddDbContext<ICoreContext, CoreContext>(x =>
        {
            /*x.UseMySql("server=localhost;port=3306;database=AutoPark;uid=root;pwd=root;", 
                serverVersion: new MySqlServerVersion(new Version(8, 4, 4)));*/
            x.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        });
    }

    public static IServiceCollection AddJwtBearerAuth(this IServiceCollection serviceCollection)
    {
        var tokenGenerator = new UserValidator();
        serviceCollection.AddSingleton<IUserValidator>(tokenGenerator);
        serviceCollection.AddAuthorization();
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = UserValidator.Issuer,
                    ValidateAudience = true,
                    ValidAudience = UserValidator.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = tokenGenerator.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                };
            });
        return serviceCollection;
    }
}