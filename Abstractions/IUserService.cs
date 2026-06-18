using WebApplication1.Models.DTOs;

namespace WebApplication1.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task BlockUsers(IEnumerable<Guid> userIds);
        Task UnblockUsers(IEnumerable<Guid> userIds);
        Task DeleteUsers(IEnumerable<Guid> userIds);
        Task DeleteUnverifiedUsers();
    }
}

