using Entities;
using Models;
using Services;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserService _userService;

        public UserBusiness(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(
            RegisterForm form)
        {
            var entity = new UserEntity
            {
                Email = form.Email,
                UserName = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                CreatedAt = DateTimeOffset.UtcNow
            };

            return await _userService.CreateUserAsync(entity, form.Password);
        }

        public async Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions)
        {
            return await _userService.GetUsersAsync(pagingOptions, sortOptions, searchOptions);
        }

        public async Task<User> GetUserAsync(ClaimsPrincipal user)
        {
            return await _userService.GetUserAsync(user);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userService.GetUserByEmailAsync(email);
        }

        public async Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal)
        {
            return await _userService.GetUserIdAsync(principal);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> AddToRoleAsync(
            Guid userId, string role)
        {
            return await _userService.AddToRoleAsync(userId, role);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> RemoveFromRoleAsync(
            Guid userId, string role)
        {
            return await _userService.RemoveFromRoleAsync(userId, role);
        }

        public async Task<PagedResults<User>> GetUsersByRoleAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            string role,
            CancellationToken ct)
        {
            return await _userService.GetUsersByRoleAsync(pagingOptions, sortOptions, searchOptions, role, ct);
        }
    }
    public interface IUserBusiness
    {
        Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(
            RegisterForm form);

        Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions);

        Task<User> GetUserAsync(ClaimsPrincipal user);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> GetUserByEmailAsync(string email);

        Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal);

        Task<(bool Succeeded, string ErrorMessage)> AddToRoleAsync(
            Guid userId, string role);

        Task<(bool Succeeded, string ErrorMessage)> RemoveFromRoleAsync(
            Guid userId, string role);

        Task<PagedResults<User>> GetUsersByRoleAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            string role,
            CancellationToken ct);
    }
}
