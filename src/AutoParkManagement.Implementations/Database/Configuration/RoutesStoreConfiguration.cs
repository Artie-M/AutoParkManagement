using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoParkManagement.Implementations.Database.Configuration;

public class RoutesStoreConfiguration : IEntityTypeConfiguration<RouteEntity>
{
    public void Configure(EntityTypeBuilder<RouteEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.StartPoint).IsRequired();
        builder.Property(e => e.FinishPoint).IsRequired();
        builder.Property(e => e.OrderTime).IsRequired();
        builder.Property(e => e.CompletionTime);
        builder.Property(e => e.Driver).IsRequired();
    }
}