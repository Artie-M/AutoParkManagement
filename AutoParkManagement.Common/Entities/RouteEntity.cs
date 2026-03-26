using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Routes")]
public class RouteEntity
{
    [Key] public int Id { get; init; }
    
    [MaxLength(128)] public required string Name { get; set; }
    
    [MaxLength(128)] public required string StartPoint { get; set; }
    
    [MaxLength(128)] public required string FinishPoint { get; set; }
}