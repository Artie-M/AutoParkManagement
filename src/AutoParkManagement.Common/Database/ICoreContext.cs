using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AutoParkManagement.Common.Database;

public interface ICoreContext
{
    DbSet<DriverEntity> Drivers { get; }
    
    DbSet<RouteEntity> Routes { get; }
    
    DbSet<VehicleEntity> Vehicles { get; }
    
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}