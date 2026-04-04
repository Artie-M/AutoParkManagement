using System.Security.Claims;
using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AutoParkManagement.Common.Auth;

public interface IUserValidator
{ 
    SymmetricSecurityKey GetSymmetricSecurityKey();
    
    string GenerateSecurityToken(string username);
    
    string GenerateSecurityToken(IEnumerable<Claim> claims);

    string GenerateSalt(int size = 32);

    string ComputeHash(string password, string salt);
    
    Task<UserEntity?> PostUserAsync(ICoreContext db, string username, string password);
    
    Task<(bool, UserEntity)> ValidateUserAsync(ICoreContext db, string username, string password);
}