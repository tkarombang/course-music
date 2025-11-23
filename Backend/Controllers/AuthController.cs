
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.DTOs.ForgotPassword;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Azure.Messaging;
using Backend.Interface;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
  private readonly IAuthInterface _authService;
  private readonly AppDbContext _context;
  private readonly IEmailInterface _emailService;

  public AuthController(AppDbContext context, IEmailInterface emailService, IAuthInterface authService)
  {
    _authService = authService;
    _context = context;
    _emailService = emailService;
  }




  [HttpPost("register")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Register([FromBody] RegisterDto dto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    try
    {
      var newUser = await _authService.RegisterAsync(dto);
      return Ok(new { message = "REGISTER BERHASIL", data = newUser });
    }
    catch (Exception err)
    {
      return BadRequest(new { message = err.Message });
    }
  }




  [HttpPost("login")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Login([FromBody] LoginDto dto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var token = await _authService.LoginAsync(dto);
    if (string.IsNullOrEmpty(token))
    {
      return Unauthorized(new { message = "EMAIL ATAU PASSWORD TIDAK VALID" });
    }

    return Ok(new { Token = token });
  }





  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null)
    {
      return Ok(new { Message = "LINK RESET TELAHDIKIRIM APABILA EMAIL TERDAFTAR" });
    }

    var resetToken = Guid.NewGuid().ToString();

    user.PasswordResetToken = resetToken;
    user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

    await _context.SaveChangesAsync();

    var frontendUrl = "http://localhost:5161";

    await _emailService.SendPasswordResetLink(user.Email, resetToken, frontendUrl);

    return Ok(new { Message = "LINK RESET TELAH DIKIRIM, APABILA EMAIL TERDAFTAR" });
  }






  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordResetToken == dto.Token);

    if (user == null || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
    {
      return BadRequest(new { Message = "PERMINTAAN RESET PASSWORD TIDAK VALID ATAU BASI" });
    }

    string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

    user.Password = newPasswordHash;
    user.PasswordResetToken = null;
    user.ResetTokenExpiry = default(DateTime);

    await _context.SaveChangesAsync();

    return Ok(new { Message = "PASSWORD BERHASIL DI GANTI, please login" });
  }




}