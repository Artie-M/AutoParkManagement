using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Maintenance")]
public class MaintenanceEntity
{
    [Key] public int Id { get; init; }
    
    [MaxLength(128)] public required string Name { get; set; }
}