using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthService
{
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;

  public AuthService(AppDbContext context, IConfiguration configuration)
  {
    _context = context;
    _configuration = configuration;
  }

  public async Task<UserModel> RegisterAsync(RegisterDto dto)
  {
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
    {
      throw new Exception("EMAIL SUDAH ADA");
    }

    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new UserModel
    {
      Username = dto.Namalengkap,
      Email = dto.Email,
      Password = hashedPassword,
      Role = dto.Role ?? "User",
      CreatedAt = DateTime.UtcNow,
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return user;
  }

  public async Task<string> LoginAsync(LoginDto dto)
  {

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null)
    {
      return null!;
    }

    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
    if (!isPasswordValid)
    {
      return null!;
    }
    return GenerateJwtToken(user);
  }

  
  private string GenerateJwtToken(UserModel user)
  {
    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.Email),
      new Claim(ClaimTypes.Role, user.Role)
    };

    var jwtKey = _configuration["Jwt:Key"];
    var jwtIssuer = _configuration["Jwt:Issuer"];
    var audience = _configuration["Jwt:Audience"];

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: jwtIssuer,
      audience: audience,
      claims: claims,
      expires: DateTime.Now.AddHours(2),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

}