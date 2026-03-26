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
    public DbSet<MaintenanceEntity> Maintenances => Set<MaintenanceEntity>();
    public DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public DbSet<TransportEntity> Transports => Set<TransportEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DriversStoreConfiguration());
        modelBuilder.ApplyConfiguration(new MaintenanceStoreConfiguration());
        modelBuilder.ApplyConfiguration(new RoutesStoreConfiguration());
        modelBuilder.ApplyConfiguration(new TransportStoreConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}