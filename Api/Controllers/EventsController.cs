using Api.Infrastructure;
using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    //Test
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventBusiness _event;
        private readonly IUserBusiness _user;
        private readonly IAuthorizationService _authzService;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly PagingOptions _defaultPagingOptions;

        public EventsController(
            IEventBusiness events,
            IUserBusiness user,
            IAuthorizationService authorizationService,
            IRazorPartialToStringRenderer renderer,
            IEmailService emailService,
            IConfiguration configuration,
            IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _event = events;
            _user = user;
            _authzService = authorizationService;
            _renderer = renderer;
            _emailService = emailService;
            _configuration = configuration;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        // GET api/events
        [HttpGet(Name = nameof(GetAllEvents))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Event>>> GetAllEvents(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var events = new PagedResults<Event>
            {
                Items = Enumerable.Empty<Event>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllEvents = await _authzService.AuthorizeAsync(
                    User, "ViewAllEventsPolicy");
                if (canSeeAllEvents.Succeeded)
                {
                    events = await _event.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    events = await _event.GetAllByByPocAsync(pagingOptions, sortOptions, searchOptions, Convert.ToInt64(User.Identity.Name.Split('@')[0]), ct);
                }
            }

            var collection = PagedCollection<Event>.Create<EventResponse>(
                Link.ToCollection(nameof(GetAllEvents)),
                events.Items.ToArray(),
                events.TotalSize,
                pagingOptions);

            //TODO
            collection.EventsQuery = FormMetadata.FromResource<Event>(
                Link.ToForm(
                    nameof(GetAllEvents),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            return collection;
        }

        // GET: api/events/true
        [HttpGet("{report:bool}", Name = nameof(GetAllEventsExcel))]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEventsExcel(
            [FromRoute] bool report,
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var events = new PagedResults<EventReport>
            {
                Items = Enumerable.Empty<EventReport>()
            };

            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllEvents = await _authzService.AuthorizeAsync(
                    User, "ViewAllEventsPolicy");
                if (canSeeAllEvents.Succeeded)
                {
                    events = await _event.GetAllForExcelAsync(pagingOptions, sortOptions, searchOptions, 0, ct);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    events = await _event.GetAllForExcelAsync(pagingOptions, sortOptions, searchOptions, Convert.ToInt64(User.Identity.Name.Split('@')[0]), ct);
                }
            }

            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(events.Items.GenerateExcel("Event"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
        }

        // GET: api/event/5
        [AllowAnonymous]
        [HttpGet("{id}", Name = nameof(GetEventById))]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(CacheProfileName = "Resource")]
        public async Task<IActionResult> GetEventById([FromRoute] Guid id, CancellationToken ct)
        {
            var eventDetail = await _event.FindAsync(id, ct);
            if (eventDetail == null) return NotFound();

            return Ok(eventDetail);
        }

        // POST: api/event
        [HttpPost(Name = nameof(CreateReminderEmail))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateReminderEmail([FromBody] ReminderEmailForm form)
        {
            var userId = await _user.GetUserIdAsync(User);
            if (userId == null) return NotFound();

            var @event = await _event.FindAsync(form.EventId, CancellationToken.None);

            foreach (var participant in @event.Participant.Where(x => !x.IsFeedbackReceived))
            {
                var feedbackType = participant.Attended ? "participated" :
                    participant.NotAttended ? "notparticipated" : "unregistered";

                var emailModel = new FeedbackRequestEmailTemplateModel
                {
                    EventName = @event.EventName,
                    EventDate = @event.EventDate.ToShortDateString(),
                    ParticipantName = participant.EmployeeName,
                    FeedbackUrl = $"{_configuration.GetSection("EmailUrl").Value}/feedback/{feedbackType}/{@event.Id}/{participant.Id}"
                };

                var body = await _renderer.RenderPartialToStringAsync("_FeedbackRequestEmailPartial", emailModel);

                await _emailService.SendAsync(User.Identity.Name, "Admin", participant.EmployeeId, $"Feedback Requested for {@event.EventName} at {@event.EventDate}", body);
            }

            return Ok();
        }
    }
}