using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly IHostingEnvironment _env;

        public RootController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]
        [ProducesResponseType(304)]
        [ResponseCache(CacheProfileName = "Static")]
        public IActionResult GetRoot()
        {
            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot)),
                Dashboard = Link.To(nameof(DashboardController.GetStats)),
                Reports = Link.ToCollection(nameof(ReportsController.GetAllEventStatus)),
                Questions = Link.ToCollection(nameof(QuestionsController.GetAllQuestions)),
                //Info = Link.To(nameof(InfoController.GetInfo)),
                Users = Link.ToCollection(nameof(UsersController.GetVisibleUsers)),
                UserInfo = Link.To(nameof(UserinfoController.Userinfo)),
                Feedback = Link.ToCollection(nameof(FeedbackController.GetAllQuestionsByFeedbackType), new { feedbackType = "participated" }),
                Events = Link.ToCollection(nameof(EventsController.GetAllEvents)),
                Token = FormMetadata.FromModel(
                    new PasswordGrantForm(),
                    Link.ToForm(nameof(TokenController.TokenExchange),
                        null, relations: Form.Relation))
            };

            return Ok(response);
        }
    }
}
