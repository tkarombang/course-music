
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace Backend.Services
{
  public class EmailService : IEmailService
  {
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _emailSettings;
    public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
    {
      _logger = logger;
      _emailSettings = emailSettings.Value;
    }

    public async Task SendPasswordResetLink(string toEmail, string resetToken, string frontendUrl)
    {
      var encodedEmail = Uri.EscapeDataString(toEmail);
      var encodedToken = Uri.EscapeDataString(resetToken);

      var resetLink = $"{frontendUrl}/newpassword?email={encodedEmail}&token={encodedToken}";

      try
      {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress("Pengguna", toEmail));
        message.Subject = "Permintaan Reset Password Anda";

        var bodyBuilder = new BodyBuilder
        {
          HtmlBody = $@"
          <p>Halo,</p>
          <p>Kami dari Tim 3 Taldio Projek Music App menerima permintaan untuk mereset password akun Anda. Silakan lanjutkan cukup dengan klik tombol di bawah:</p>
          <div style='text-align: center; margin: 20px 0;'>
          <a href='{resetLink}' style='
                background-color: #226957;
                color: white !important;
                padding: 10px 20px;
                text-align: center;
                text-decoration: none;
                display: inline-block;
                border-radius: 5px;
                font-weight: bold;
                font-size: 16px;
                min-width: 200px;
                mso-hide: all;
            '>
                RESET PASSWORD
            </a>
        </div>
        <p>Link ini akan mengarahkan Anda ke halaman pengaturan password baru. Jika Anda tidak meminta reset password, abaikan email ini.</p>
        <p>Hormat kami Tim Tiga - Tiga Laki-laki Pemberani ganteng dan manis,<br>{_emailSettings.SenderName}</p>",
          TextBody = $"Untuk mereset password, salin dan tempel tautan berikut: {resetLink}"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
          await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

          await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

          await client.SendAsync(message);
          await client.DisconnectAsync(true);
        }
        _logger.LogInformation($"[EMAIL TERKIRIM] Berhasil ke: {toEmail}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"[EMAIL GAGAL] Gagal mengirim email ke {toEmail}. Pastikan App Password dan 2FA sudah benar. Error: {ex.Message}");
        throw;
      }
    }
  }
}