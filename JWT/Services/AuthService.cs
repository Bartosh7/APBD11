﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWT.Contexts;
using JWT.Models;
using JWT.RequestModel;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginResponseModel = JWT.Controllers.LoginResponseModel;

namespace JWT.Services;

public interface IAuthService
{
Task<LoginResponseModel> LoginAsync(LoginAndRegisterRequestModel model);
Task<LoginResponseModel> RefreshTokenAsync(string refreshToken);
Task<bool> RegisterUserAsync(LoginAndRegisterRequestModel model);
}

public class AuthService(DatabaseContext context, IConfiguration configuration) : IAuthService
{


public async Task<LoginResponseModel> RefreshTokenAsync(string existingRefreshToken)
{
    var refreshTokenRecord = await context.RefreshTokens.Include(rt => rt.User)
        .SingleOrDefaultAsync(rt => rt.Token == existingRefreshToken);

    if (refreshTokenRecord != null && refreshTokenRecord.ToDate > DateTime.Now)
    {
        AppUser associatedUser = refreshTokenRecord.User;
        var jwtToken = GenerateJwtToken(associatedUser);
        var refreshToken = GenerateRefreshToken();

        refreshTokenRecord.Token = refreshToken;
        refreshTokenRecord.ToDate = DateTime.Now.AddDays(3);
        context.RefreshTokens.Update(refreshTokenRecord);
        await context.SaveChangesAsync();

        return new LoginResponseModel
        {
            Token = jwtToken,
            RefreshToken = refreshToken
        };
    }

    return null;
}


public async Task<LoginResponseModel> LoginAsync(LoginAndRegisterRequestModel model)
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
    if (user != null && VerifyPassword(user.PasswordHashed, model.Password, user.Salt))
    {
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        context.RefreshTokens.Add(new LogToken
        {
            Token = refreshToken,
            ToDate = DateTime.Now.AddDays(3),
            UserId = user.IdUser
        });
        await context.SaveChangesAsync();
        return new LoginResponseModel
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
    return null;
}



public async Task<bool> RegisterUserAsync(LoginAndRegisterRequestModel model)
{
    if (await context.Users.AnyAsync(u => u.Email == model.Email))
        return false;
    
    var (hashedPassword, salt) = GetHashedPasswordAndSalt(model.Password);
    
    var user = new AppUser
    {
        Email = model.Email,
        PasswordHashed = hashedPassword,
        Salt = salt
        
    };
    
    context.Users.Add(user);
    await context.SaveChangesAsync();

    return true;
}

public static Tuple<string, string> GetHashedPasswordAndSalt(string password)
{
    byte[] salt = new byte[128 / 8];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(salt);
    }

    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

    string saltBase64 = Convert.ToBase64String(salt);

    return new(hashed, saltBase64);
}


private bool VerifyPassword(string storedHash, string password, string storedSalt)
{
    var salt = Convert.FromBase64String(storedSalt);
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));
    return hashed == storedHash;
}

private string GenerateJwtToken(AppUser user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
        Expires = DateTime.Now.AddMinutes(10),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        Issuer = configuration["JWT:Issuer"],
        Audience = configuration["JWT:Audience"]
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

public static string GenerateRefreshToken()
{
    var randomNumber = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
}