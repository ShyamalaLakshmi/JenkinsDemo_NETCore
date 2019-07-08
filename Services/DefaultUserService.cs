using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultUserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultUserService(
            UserManager<UserEntity> userManager,
            IConfigurationProvider mappingConfiguration)
        {
            _userManager = userManager;
            _mappingConfiguration = mappingConfiguration;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(
            UserEntity user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return (false, firstError);
            }

            return (true, user.Id.ToString());
        }

        public async Task<PagedResults<User>> GetUsersAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions)
        {
            IQueryable<UserEntity> query = _userManager.Users.AsNoTracking();
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync();

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<User>(_mappingConfiguration)
                .ToArrayAsync();

            return new PagedResults<User>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<PagedResults<User>> GetUsersByRoleAsync(
            PagingOptions pagingOptions,
            SortOptions<User, UserEntity> sortOptions,
            SearchOptions<User, UserEntity> searchOptions,
            string role,
            CancellationToken ct)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            IQueryable<UserEntity> query = users.AsQueryable();
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = query.Count();

            var items = query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<User>(_mappingConfiguration)
                .ToArray();

            return new PagedResults<User>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<User> GetUserAsync(ClaimsPrincipal user)
        {
            var entity = await _userManager.GetUserAsync(user);
            var mapper = _mappingConfiguration.CreateMapper();
            
            return mapper.Map<User>(entity);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var entity = await _userManager.Users.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == userId);
            var mapper = _mappingConfiguration.CreateMapper();

            return mapper.Map<User>(entity);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var entity = await _userManager.Users.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == email);
            var mapper = _mappingConfiguration.CreateMapper();

            return mapper.Map<User>(entity);
        }

        public async Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal)
        {
            var entity = await _userManager.GetUserAsync(principal);
            if (entity == null) return null;

            return entity.Id;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> AddToRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return (false, firstError);
            }

            await _userManager.UpdateAsync(user);

            return (true, null);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> RemoveFromRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return (false, firstError);
            }

            await _userManager.UpdateAsync(user);

            return (true, null);
        }

    }
}
