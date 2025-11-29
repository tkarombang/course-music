using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
  public class RefreshTokenModel
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }

    [Required]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiredAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked => RevokedAt.HasValue;

    public string? ReplaceByToken { get; set; }

    [MaxLength(200)]
    public string? CreatedByIp { get; set; }
  }
}