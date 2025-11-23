using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.DTOs.ForgotPassword;
using Backend.Exceptions;
using Backend.Interface;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthInterface
{
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;
  private readonly ILogger<AuthService> _logger;
  private readonly IEmailInterface _emailService;

  public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger, IEmailInterface emailService)
  {
    _context = context;
    _configuration = configuration;
    _logger = logger;
    _emailService = emailService;
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







  public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null)
    {
      _logger.LogWarning("USER TIDAK DITEMUKAN DENGAN EMAIL: {Email}", dto.Email);
      throw new NotFoundException($"USER NOT FOUND WITH EMAIL: {dto.Email}");
    }

    var resetToken = Guid.NewGuid().ToString();

    user.PasswordResetToken = resetToken;
    user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

    await _context.SaveChangesAsync();
    var frontendUrl = "http://localhost:5161";

    await _emailService.SendPasswordResetLink(user.Email, resetToken, frontendUrl);

    return resetToken;

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

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
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