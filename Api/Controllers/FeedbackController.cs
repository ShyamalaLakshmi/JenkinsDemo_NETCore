using Business;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackBusiness _feedback;
        private readonly IUserBusiness _user;
        private readonly PagingOptions _defaultPagingOptions;

        public FeedbackController(IFeedbackBusiness feedback, IUserBusiness user, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _feedback = feedback;
            _user = user;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        // GET api/feedback/participated
        [HttpGet("{feedbackType}", Name = nameof(GetAllQuestionsByFeedbackType))]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        //[ResponseCache(CacheProfileName = "Collection")]
        public async Task<ActionResult<Collection<Question>>> GetAllQuestionsByFeedbackType(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Question, QuestionEntity> sortOptions,
            [FromQuery] SearchOptions<Question, QuestionEntity> searchOptions,
            [FromRoute] string feedbackType,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var questions = await _feedback.GetAllByFeedbackTypeAsync(pagingOptions, sortOptions, searchOptions, feedbackType, ct);

            var collection = PagedCollection<Question>.Create<QuestionResponse>(
                Link.ToCollection(nameof(GetAllQuestionsByFeedbackType)),
                questions.Items.ToArray(),
                questions.TotalSize,
                pagingOptions);

            //TODO
            //collection.Answers = Link.ToCollection(nameof(AnswersController.GetItem));
            collection.QuestionForm = FormMetadata.FromModel(
                new QuestionForm(),
                Link.ToForm(nameof(QuestionsController.CreateQuestion),
                    null, relations: Form.Relation));
            collection.QuestionsQuery = FormMetadata.FromResource<Question>(
                Link.ToForm(
                    nameof(GetAllQuestionsByFeedbackType),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            return collection;
        }

        // POST: api/feedback
        [HttpPost(Name = nameof(CreateFeedback))]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateFeedback(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Question, QuestionEntity> sortOptions,
            [FromQuery] SearchOptions<Question, QuestionEntity> searchOptions,
            [FromBody] FeedbackForm feedback)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            //var userId = await _user.GetUserIdAsync(User);
            //if (userId == null) return NotFound();

            await _feedback.AddAsync(pagingOptions, sortOptions, searchOptions, feedback);

            return Ok();
        }
    }
}