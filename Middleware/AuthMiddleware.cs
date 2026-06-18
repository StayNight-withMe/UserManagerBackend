using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Enums;
using WebApplication1.Persistence.Context;

namespace WebApplication1.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/api/identity"))
            {
                await _next(context);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null || user.Status == UserStatus.blocked)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next(context);
        }
    }
}

