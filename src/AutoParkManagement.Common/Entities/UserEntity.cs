using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParkManagement.Common.Entities;

[Table("Users")]
public class UserEntity
{
    public int Id { get; init; }
    
    [MaxLength(64)] public required string Name { get; init; }
    
    [MaxLength(128)] public string? Email { get; set; }
    
    [MaxLength(64)] public required string Salt { get; init; }
    
    [MaxLength(128)] public required string Password { get; set; }
}