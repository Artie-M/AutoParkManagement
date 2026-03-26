using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Drivers")]
public class DriverEntity
{
    [Key] public int Id { get; init; }
    
    [MaxLength(128)] public required string Name { get; set; }
    
    public int Rating { get; set; }
    
    public int CarId { get; set; }
    
    public int Order { get; set; }
}