using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoParkManagement.Common.Auth;
using AutoParkManagement.Common.Database;
using AutoParkManagement.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoParkManagement.Implementations.Auth;

public class UserValidator : IUserValidator
{
    public const string Issuer = "AutoParkServer";
    public const string Audience = "AutoParkClient";
    private readonly string _key = "mysupersecret_secretsecretsecretkey!123";
    
    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
    }
    
    public string GenerateSecurityToken(string username)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
        return GenerateSecurityToken(claims);
    }
    
    public string GenerateSecurityToken(IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateSalt(int size = 32)
    {
        var buffer = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }
        return Convert.ToBase64String(buffer);
    }

    public string ComputeHash(string password, string salt)
    {
        var combinedPassword = password + salt;
        var bytes = Encoding.UTF8.GetBytes(combinedPassword);

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public async Task<UserEntity?> PostUserAsync(ICoreContext db, string username, string password)
    {
        var salt = GenerateSalt();
        var hash = ComputeHash(password, salt);
        var user = new UserEntity
        {
            Name = username,
            Salt = salt,
            Password = hash
        };
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<(bool, UserEntity)> ValidateUserAsync(ICoreContext db, string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Name == username);

        if (user == null ||
            user.Password != ComputeHash(password, user.Salt))
        {
            return (false, null!);
        }
        
        return (true, user);
    }
}