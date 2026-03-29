using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Vehicles")]
public class VehicleEntity
{
    [Key] public uint Id { get; init; }
    
    [MaxLength(128)] public required string Name { get; set; }
    
    [MaxLength(16)] public required string CarNumber { get; set; }
}