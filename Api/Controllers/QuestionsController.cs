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
    [Authorize(Roles = "Admin")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionBusiness _question;
        private readonly IUserBusiness _user;
        private readonly PagingOptions _defaultPagingOptions;

        public QuestionsController(IQuestionBusiness question, IUserBusiness user, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _question = question;
            _user = user;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        // GET api/questions
        [HttpGet(Name = nameof(GetAllQuestions))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Question>>> GetAllQuestions(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Question, QuestionEntity> sortOptions,
            [FromQuery] SearchOptions<Question, QuestionEntity> searchOptions,
            CancellationToken ct)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var questions = await _question.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);

            var collection = PagedCollection<Question>.Create<QuestionResponse>(
                Link.ToCollection(nameof(GetAllQuestions)),
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
                    nameof(GetAllQuestions),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            return collection;
        }

        // GET: api/question/5
        [HttpGet("{id}", Name = nameof(GetQuestionById))]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(CacheProfileName = "Resource")]
        public async Task<IActionResult> GetQuestionById([FromRoute] Guid id, CancellationToken ct)
        {
            var question = await _question.FindAsync(id, ct);
            if (question == null) return NotFound();

            return Ok(question);
        }

        // PUT: api/question/5
        [HttpPut("{id}", Name = nameof(UpdateQuestion))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateQuestion([FromRoute] Guid id, [FromBody] QuestionForm question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            var userId = await _user.GetUserIdAsync(User);
            if (userId == null) return NotFound();

            try
            {
                await _question.UpdateAsync(question, userId.Value);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/question
        [HttpPost(Name = nameof(CreateQuestion))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionForm question)
        {
            var userId = await _user.GetUserIdAsync(User);
            if (userId == null) return NotFound();

            var questionId = await _question.AddAsync(question, userId.Value);

            //return CreatedAtAction(nameof(GetAllQuestions), new
            //{
            //    id = question.Id
            //}, question);

            return Created(
                Url.Link(nameof(GetQuestionById),
                new { id = questionId }),
                null);
        }

        // DELETE: api/question/5
        [HttpDelete("{id}", Name = nameof(DeleteQuestion))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteQuestion([FromRoute] Guid id)
        {
            if (!await QuestionExists(id))
            {
                return NotFound();
            }

            await _question.RemoveAsync(id);

            return Ok();
        }

        private async Task<bool> QuestionExists(Guid id)
        {
            return await _question.ExistsAsync(id);
        }
    }
}