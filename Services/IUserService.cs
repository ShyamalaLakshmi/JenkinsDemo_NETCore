using Entities;
using Models;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions);

        Task<PagedResults<User>> GetUsersByRoleAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            string role,
            CancellationToken ct);

        Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(UserEntity user, string password);

        Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserAsync(ClaimsPrincipal user);

        Task<(bool Succeeded, string ErrorMessage)> AddToRoleAsync(Guid userId, string role);

        Task<(bool Succeeded, string ErrorMessage)> RemoveFromRoleAsync(Guid userId, string role);
    }
}
