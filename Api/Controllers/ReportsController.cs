using Api.Infrastructure;
using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportBusiness _report;
        private readonly IUserBusiness _user;
        private readonly IAuthorizationService _authzService;
        private readonly PagingOptions _defaultPagingOptions;

        public ReportsController(IReportBusiness report, IUserBusiness user, IAuthorizationService authorizationService, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _report = report;
            _user = user;
            _authzService = authorizationService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        // GET api/questions
        [HttpGet(Name = nameof(GetAllEventStatus))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<EventStatus>>> GetAllEventStatus(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var events = new PagedResults<EventStatus>
            {
                Items = Enumerable.Empty<EventStatus>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllEvents = await _authzService.AuthorizeAsync(
                    User, "ViewAllEventsPolicy");
                if (canSeeAllEvents.Succeeded)
                {
                    events = await _report.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    events = await _report.GetAllByPocAsync(pagingOptions, sortOptions, searchOptions, Convert.ToInt64(User.Identity.Name.Split('@')[0]), ct);
                }
            }

            var collection = PagedCollection<EventStatus>.Create<EventStatusResponse>(
                Link.ToCollection(nameof(GetAllEventStatus)),
                events.Items.ToArray(),
                events.TotalSize,
                pagingOptions);

            //TODO
            collection.EventsQuery = FormMetadata.FromResource<EventStatus>(
                Link.ToForm(
                    nameof(GetAllEventStatus),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            return collection;
        }

        [HttpGet("{report:bool}", Name = nameof(GetAllEventStatusReport))]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEventStatusReport(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            [FromQuery] bool report,
            CancellationToken ct)
        {
            pagingOptions.Offset = _defaultPagingOptions.Offset;
            pagingOptions.Limit = _defaultPagingOptions.Limit;

            var events = new PagedResults<EventStatusReport>
            {
                Items = Enumerable.Empty<EventStatusReport>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllEvents = await _authzService.AuthorizeAsync(
                    User, "ViewAllEventsPolicy");
                if (canSeeAllEvents.Succeeded)
                {
                    events = await _report.GetAllReportAsync(pagingOptions, sortOptions, searchOptions, ct);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    events = await _report.GetAllReportByPocAsync(pagingOptions, sortOptions, searchOptions, Convert.ToInt64(User.Identity.Name.Split('@')[0]), ct);
                }
            }

            return File(events.Items.GenerateExcel("EventStatus"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EventStatus.xlsx");
        }
    }
}