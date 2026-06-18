using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Enums;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.Entities;
using WebApplication1.Persistence.Context;
using Microsoft.Extensions.Options;
using WebApplication1.Abstractions;
using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtUtil _jwtUtil;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppConfig _appConfig;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            ApplicationDbContext context,
            IJwtUtil jwtUtil,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            IOptions<AppConfig> appConfig,
            ILogger<IdentityService> logger)
        {
            _context = context;
            _jwtUtil = jwtUtil;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _appConfig = appConfig.Value;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? Token)> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || user.Status == UserStatus.blocked)
            {
                return (false, "Invalid credentials or account blocked", null);
            }

            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return (false, "Invalid credentials", null);
            }

            if (user.Status != UserStatus.blocked)
            {
                user.LastLoginTime = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            var token = _jwtUtil.GenerateToken(user);
            return (true, "Login successful", token);
        }

        public async Task<(bool Success, string Message)> Register(RegisterDto dto)
        {
            _logger.LogInformation("Starting Register method.");
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                _logger.LogInformation("User already exists.");
                return (false, "User with this email already exists.");
            }

            _logger.LogInformation("Hashing password.");
            var passwordHash = _passwordHasher.HashPassword(dto.Password);
            _logger.LogInformation("Password hashed.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Status = UserStatus.unverified,
                CreatingDate = DateTime.UtcNow,
                LastLoginTime = DateTime.UtcNow,
                VerificationToken = Guid.NewGuid().ToString()
            };

            _logger.LogInformation("Attempting to add user to context.");
            _context.Users.Add(user);
            _logger.LogInformation("Attempting to save changes.");
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved new user to database.");

            var verificationLink = $"{_appConfig.BaseUrl}/api/v1/identity/verify-email?email={user.Email}&token={user.VerificationToken}";
            
            try
            {
                _logger.LogInformation("Attempting to send verification email to {Email}", user.Email);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _emailService.SendEmail(user.Email, "Verify your email", 
                    $"<a href='{verificationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a>", cts.Token);
                _logger.LogInformation("Sent verification email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification email");
            }

            return (true, "User registered. Check your email.");
        }

        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.VerificationToken == token);
            if (user == null)
            {
                return false;
            }

            user.Status = UserStatus.active;
            user.VerificationToken = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

