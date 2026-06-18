using Microsoft.EntityFrameworkCore;
using WebApplication1.Abstractions;
using WebApplication1.Common.Enums;
using WebApplication1.Models.DTOs;
using WebApplication1.Persistence.Context;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    LastLoginTime = u.LastLoginTime,
                    Status = u.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task BlockUsers(IEnumerable<Guid> userIds)
        {
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                if (user.Status != UserStatus.blocked)
                {
                    user.PreviousStatus = user.Status;
                }
                user.Status = UserStatus.blocked;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UnblockUsers(IEnumerable<Guid> userIds)
        {
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.Status = user.PreviousStatus ?? UserStatus.unverified;
                user.PreviousStatus = null;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUsers(IEnumerable<Guid> userIds)
        {
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUnverifiedUsers()
        {
            var unverified = await _context.Users.Where(u => u.Status == UserStatus.unverified).ToListAsync();
            _context.Users.RemoveRange(unverified);
            await _context.SaveChangesAsync();
        }
    }
}

