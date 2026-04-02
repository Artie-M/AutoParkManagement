using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoParkManagement.EndpointManagers;

public class RoutesStoreEndpointManager
{
    public static RoutesStoreEndpointManager Singleton { get; private set; }

    static RoutesStoreEndpointManager()
    {
        Singleton = new RoutesStoreEndpointManager();
    }
    
    private WebApplication _app;
    
    public void RegisterEndpoints(WebApplication app)
    {
        _app = app;
        app.MapGet("api/routes/", GetAllRoutesAsync);
        app.MapGet("api/routes/{id:min(0)}", GetRouteAsync);
        app.MapDelete("api/routes/{id:min(0)}", DeleteRouteAsync);
        app.MapPut("api/routes/{id:min(0)}", PutRouteAsync);
        app.MapPost("api/routes/", PostRouteAsync);
    }

    private async Task GetAllRoutesAsync(HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        httpContext.Response.ContentType = "application/json; charset=utf-8";
        httpContext.Response.Headers.ContentLanguage = "ru-RU";
        
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsJsonAsync(await db.Routes.ToListAsync());
    }
    
    private async Task GetRouteAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        var driver = await db.Routes.FindAsync(id);
        
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
    
    private async Task DeleteRouteAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var route = await db.Routes.FindAsync(id);
        if (route == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        db.Routes.Remove(route);
        await db.SaveChangesAsync();
        httpContext.Response.StatusCode = 204; // No Content
    }

    private async Task PutRouteAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var updatedRoute = await httpContext.Request.ReadFromJsonAsync<RouteEntity>();
        if (updatedRoute == null || id != updatedRoute.Id)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        var existingRoute = await db.Routes.FindAsync(id);
        if (existingRoute == null)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        existingRoute.Name = updatedRoute.Name;
        existingRoute.Driver = updatedRoute.Driver;
        existingRoute.StartPoint = updatedRoute.StartPoint;
        existingRoute.FinishPoint = updatedRoute.FinishPoint;
        existingRoute.OrderTime = updatedRoute.OrderTime;
        existingRoute.CompletionTime = updatedRoute.CompletionTime;

        await db.SaveChangesAsync();
        httpContext.Response.StatusCode = 204;
    }

    private async Task PostRouteAsync(uint id, HttpContext httpContext)
    {
        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ICoreContext>();
        
        var newRoute = await httpContext.Request.ReadFromJsonAsync<RouteEntity>();
        if (newRoute == null)
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            return;
        }

        await db.Routes.AddAsync(newRoute);
        await db.SaveChangesAsync();
        
        httpContext.Response.StatusCode = 201; // Created
        await httpContext.Response.WriteAsJsonAsync(newRoute);
    }
}