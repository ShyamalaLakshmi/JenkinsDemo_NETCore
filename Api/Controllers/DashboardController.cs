using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardBusiness _dashboard;
        private readonly IUserBusiness _user;
        private readonly IAuthorizationService _authzService;

        public DashboardController(
            IDashboardBusiness dashboard,
            IUserBusiness user,
            IAuthorizationService authorizationService)
        {
            _dashboard = dashboard;
            _user = user;
            _authzService = authorizationService;
        }

        // GET: api/dashboard/5
        [Route("api/dashboard/GetStats")]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dashboard>> GetStats(CancellationToken ct)
        {
            var dashboard = new Dashboard();

            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllEvents = await _authzService.AuthorizeAsync(
                    User, "ViewAllEventsPolicy");
                if (canSeeAllEvents.Succeeded)
                {
                    dashboard = await _dashboard.GetStatsAsync(0, ct);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    dashboard = await _dashboard.GetStatsAsync(Convert.ToInt64(User.Identity.Name.Split('@')[0]), ct);
                }
            }

            return dashboard;
        }
    }
}