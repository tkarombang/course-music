namespace Backend.Interface
{
  public interface IEmailInterface
  {
    Task SendPasswordResetLink(string toEmail, string resetToken, string frontendLink);
  }
}