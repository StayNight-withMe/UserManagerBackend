using WebApplication1.Common.Enums;

namespace WebApplication1.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }
        public UserStatus Status { get; set; }
        public UserStatus? PreviousStatus { get; set; }
        public required string Email { get; set; }
        public DateTime CreatingDate {  get; set; }
        public DateTime LastLoginTime { get; set; }
        public string? VerificationToken { get; set; }
    }
}

