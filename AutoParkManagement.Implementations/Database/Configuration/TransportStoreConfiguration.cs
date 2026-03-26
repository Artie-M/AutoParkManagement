using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoParkManagement.Implementations.Database.Configuration;

public class TransportStoreConfiguration : IEntityTypeConfiguration<TransportEntity>
{
    public void Configure(EntityTypeBuilder<TransportEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Maintenance).IsRequired();
        builder.Property(e => e.CarNumber).IsRequired();
    }
}