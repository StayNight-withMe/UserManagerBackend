using WebApplication1.Models.DTOs;

namespace WebApplication1.Abstractions
{
    public interface IIdentityService
    {
        Task<(bool Success, string Message, string? Token)> Login(LoginDto dto);
        Task<(bool Success, string Message)> Register(RegisterDto dto);
        Task<bool> VerifyEmail(string email, string token);
    }
}

