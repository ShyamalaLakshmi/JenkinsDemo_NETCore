using System;
using AspNet.Security.OpenIdConnect.Primitives;
using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserinfoController : ControllerBase
    {
        private readonly IUserBusiness _user;

        public UserinfoController(IUserBusiness user)
        {
            _user = user;
        }

        // GET /userinfo
        [HttpGet(Name = nameof(Userinfo))]
        [ProducesResponseType(401)]
        public async Task<ActionResult<UserinfoResponse>> Userinfo()
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user does not exist."
                });
            }
            var userId = await _user.GetUserIdAsync(User);

            return new UserinfoResponse
            {
                Self = Link.To(nameof(Userinfo)),
                GivenName = user.FirstName,
                FamilyName = user.LastName,
                Role = User.IsInRole("Admin") ? string.Empty : Guid.NewGuid().ToString(),
                Subject = Url.Link(
                    nameof(UsersController.GetUserById),
                    new { userId })
            };
        }
    }
}