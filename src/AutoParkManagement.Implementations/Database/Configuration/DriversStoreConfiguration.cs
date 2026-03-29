using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoParkManagement.Implementations.Database.Configuration;

public class DriversStoreConfiguration : IEntityTypeConfiguration<DriverEntity>
{
    public void Configure(EntityTypeBuilder<DriverEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Rating).IsRequired();
        builder.Property(e => e.CarId).IsRequired();
    }
}