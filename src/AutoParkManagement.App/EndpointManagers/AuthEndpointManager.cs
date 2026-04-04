using System.Security.Claims;
using AutoParkManagement.Common.Auth;
using AutoParkManagement.Common.Database;

namespace AutoParkManagement.EndpointManagers;

public class AuthEndpointManager
{
    public static AuthEndpointManager Singleton { get; private set; }

    static AuthEndpointManager()
    {
        Singleton = new AuthEndpointManager();
    }
    
    private WebApplication _app;
    
    public void RegisterEndpoints(WebApplication app)
    {
        _app = app;
        app.MapPost("/api/auth/login", PostAuthAsync);
    }

    private async Task PostAuthAsync(HttpRequest request)
    {
        var context = request.HttpContext;

        var loginData = await request.ReadFromJsonAsync<LoginDto>();
        if (loginData == null || string.IsNullOrEmpty(loginData.Username))
        {
            context.Response.StatusCode = 400; // Bad Request
            return;
        }

        using var scope = _app.Services.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IUserValidator>();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();

        var (success, user) = await validator.ValidateUserAsync(db, loginData.Username, loginData.Password);

        if (!success)
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsJsonAsync(new { error = "Неверные учетные данные" });
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("id", user.Id.ToString())
        };

        if (user.Email != null)
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }
        
        var token = validator.GenerateSecurityToken(claims);
        context.Response.StatusCode = 200;
        await context.Response.WriteAsJsonAsync(new { token = token });
    }
}

record LoginDto(string Username, string Password);