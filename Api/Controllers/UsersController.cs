using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserBusiness _user;
        private readonly PagingOptions _defaultPagingOptions;
        private readonly IAuthorizationService _authzService;

        public UsersController(
            IUserBusiness userBusiness,
            IOptions<PagingOptions> defaultPagingOptions,
            IAuthorizationService authorizationService)
        {
            _user = userBusiness;
            _defaultPagingOptions = defaultPagingOptions.Value;
            _authzService = authorizationService;
        }

        [Authorize]
        [HttpGet(Name = nameof(GetVisibleUsers))]
        public async Task<ActionResult<PagedCollection<User>>> GetVisibleUsers(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<User, UserEntity> sortOptions,
            [FromQuery] SearchOptions<User, UserEntity> searchOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var users = new PagedResults<User>
            {
                Items = Enumerable.Empty<User>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeEveryone = await _authzService.AuthorizeAsync(
                    User, "ViewAllUsersPolicy");
                if (canSeeEveryone.Succeeded)
                {
                    users = await _user.GetUsersAsync(
                        pagingOptions, sortOptions, searchOptions);
                }
                else
                {
                    var myself = await _user.GetUserAsync(User);
                    users.Items = new[] { myself };
                    users.TotalSize = 1;
                }
            }

            var collection = PagedCollection<User>.Create<UsersResponse>(
                Link.ToCollection(nameof(GetVisibleUsers)),
                users.Items.ToArray(),
                users.TotalSize,
                pagingOptions);

            collection.Me = Link.To(nameof(UserinfoController.Userinfo));
            collection.Register = FormMetadata.FromModel(
                new RegisterForm(),
                Link.ToForm(nameof(RegisterUser), relations: Form.CreateRelation));

            return collection;
        }

        [Authorize]
        [Route("/api/[controller]/Pmo")]
        public async Task<ActionResult<PagedCollection<User>>> GetUsersByRole(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<User, UserEntity> sortOptions,
            [FromQuery] SearchOptions<User, UserEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var users = new PagedResults<User>
            {
                Items = Enumerable.Empty<User>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeEveryone = await _authzService.AuthorizeAsync(
                    User, "ViewAllUsersPolicy");
                if (canSeeEveryone.Succeeded)
                {
                    users = await _user.GetUsersByRoleAsync(
                        pagingOptions, sortOptions, searchOptions, "Pmo", ct);
                }
            }

            var collection = PagedCollection<User>.Create<UsersResponse>(
                Link.ToCollection(nameof(GetVisibleUsers)),
                users.Items.ToArray(),
                users.TotalSize,
                pagingOptions);

            collection.Me = Link.To(nameof(UserinfoController.Userinfo));
            collection.Register = FormMetadata.FromModel(
                new RegisterForm(),
                Link.ToForm(nameof(RegisterUser), relations: Form.CreateRelation));

            return collection;
        }

        [Authorize]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public async Task<ActionResult<User>> GetUserById(Guid userId)
        {
            var currentUserId = await _user.GetUserIdAsync(User);
            if (currentUserId == null) return NotFound();

            if (currentUserId == userId)
            {
                var myself = await _user.GetUserAsync(User);
                return myself;
            }

            var canSeeEveryone = await _authzService.AuthorizeAsync(
                User, "ViewAllUsersPolicy");
            if (!canSeeEveryone.Succeeded) return NotFound();

            var user = await _user.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            return user;
        }

        // POST /users
        [HttpPost(Name = nameof(RegisterUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> RegisterUser(
            [FromBody] RegisterForm form)
        {
            var (succeeded, message) = await _user.CreateUserAsync(form);
            if (succeeded) return Created(
                Url.Link(nameof(UserinfoController.Userinfo), null),
                null);

            return BadRequest(new ApiError
            {
                Message = "Registration failed.",
                Detail = message
            });
        }

        // PUT: api/question/5
        [Authorize(Roles = "Admin")]
        [HttpPut(Name = nameof(UpdateRole))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRole([FromBody] RoleForm role)
        {
            var user = await _user.GetUserByEmailAsync(role.Email);
            if (user == null) return NotFound();

            try
            {
                if (role.Add)
                    await _user.AddToRoleAsync(user.Id, "Pmo");
                else
                    await _user.RemoveFromRoleAsync(user.Id, "Pmo");
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!await UserExists(user.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
                throw;
            }

            return NoContent();
        }
    }
}