using AutoParkManagement.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace AutoParkManagement.EndpointManagers;

public class DriverStoreEndpointManager
{
    public static DriverStoreEndpointManager Singleton { get; private set; }

    private WebApplication _app;
    
    static DriverStoreEndpointManager()
    {
        Singleton = new DriverStoreEndpointManager();
    }
    
    public void RegisterEndpoints(WebApplication app)
    {
        _app = app;
        app.MapGet("api/drivers/", GetAllDriversAsync);
        app.MapGet("api/drivers/{id:int}", GetDriverAsync);
    }

    private async Task GetAllDriversAsync(HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsJsonAsync(await db.Drivers.ToListAsync());
    }
    
    private async Task GetDriverAsync(int id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        var driver = await db.Drivers.FindAsync(id);
        
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        if (driver == null)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync("Not Found");
            return;
        }
        
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsJsonAsync(driver);
    }
}