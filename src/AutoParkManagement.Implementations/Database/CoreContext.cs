using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using AutoParkManagement.Implementations.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AutoParkManagement.Implementations.Database;

public class CoreContext : DbContext, ICoreContext
{
    public CoreContext(DbContextOptions<CoreContext> options) : base(options)
    {
    }
    
    public DbSet<DriverEntity> Drivers => Set<DriverEntity>();
    public DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DriversStoreConfiguration());
        modelBuilder.ApplyConfiguration(new RoutesStoreConfiguration());
        modelBuilder.ApplyConfiguration(new VehiclesStoreConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}