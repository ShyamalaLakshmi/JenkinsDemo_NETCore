using Api.Infrastructure;
using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TaskController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly IEventBusiness _event;
        private readonly IReportBusiness _report;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IEmailService _emailService;
        private readonly PagingOptions _defaultPagingOptions;
        private readonly IAuthorizationService _authzService;
        private readonly IUserBusiness _user;
        private readonly IConfiguration _configuration;

        public TaskController(
            IHostingEnvironment env,
            IEventBusiness eventBusiness,
            IReportBusiness report,
            IRazorPartialToStringRenderer renderer,
            IEmailService emailService,
            IAuthorizationService authorizationService,
            IUserBusiness user,
            IConfiguration configuration,
            IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _env = env;
            _event = eventBusiness;
            _report = report;
            _renderer = renderer;
            _emailService = emailService;
            _authzService = authorizationService;
            _user = user;
            _configuration = configuration;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        [HttpGet(Name = nameof(SendReminderEmailForAllEvents))]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendReminderEmailForAllEvents(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var events = await _event.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);

            foreach (var @event in events.Items)
            {
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
            }

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> EmailEventStatusReport(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Event, EventEntity> sortOptions,
            [FromQuery] SearchOptions<Event, EventEntity> searchOptions,
            [FromBody] ReportEmailForm form)
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
                    events = await _report.GetAllReportAsync(pagingOptions, sortOptions, searchOptions, CancellationToken.None);
                }
                else
                {
                    var userId = await _user.GetUserIdAsync(User);
                    events = await _report.GetAllReportByPocAsync(pagingOptions, sortOptions, searchOptions, Convert.ToInt64(User.Identity.Name.Split('@')[0]), CancellationToken.None);
                }
            }

            events.Items.SaveExcel("EventRatingReport", $@"{_env.WebRootPath}\EventRatingReport");

            await _emailService.SendAsync(User.Identity.Name, "Admin", form.Email.Split('@')[0],
                "Event Status Report", $"Dear {form.Email.Split('@')[0]}, <br/> Event Rating Report. <br/> *This is an automatically generated email, please do not reply*",
                $@"{_env.WebRootPath}\EventRatingReport\EventRatingReport.xlsx");

            return Ok();
        }
    }
}