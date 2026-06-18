namespace WebApplication1.Models.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LastLoginTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

