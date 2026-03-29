using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Drivers")]
public class DriverEntity
{
    [Key] public uint Id { get; init; }
    
    [MaxLength(128)] public required string Name { get; set; }
    
    public uint Rating { get; set; }
    
    public uint CarId { get; set; }
}