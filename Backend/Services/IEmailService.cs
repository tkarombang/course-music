namespace Backend.Services
{
  public interface IEmailService
  {
    Task SendPasswordResetLink(string toEmail, string resetToken, string frontendLink);
    
  }
}