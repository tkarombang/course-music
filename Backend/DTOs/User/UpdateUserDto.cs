namespace Backend.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        // public string? Password { get; set; }
        // public string? Role { get; set; }
    }
}
