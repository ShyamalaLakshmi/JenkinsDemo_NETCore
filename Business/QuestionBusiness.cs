using AutoMapper;
using Entities;
using Microsoft.Extensions.Options;
using Models;
using Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business
{
    public class QuestionBusiness : IQuestionBusiness
    {
        private readonly IQuestionService _questionService;
        private readonly IMapper _mapper;
        private readonly PagingOptions _defaultPagingOptions;

        public QuestionBusiness(IQuestionService questionService, IConfigurationProvider mappingConfiguration, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _questionService = questionService;
            _mapper = mappingConfiguration.CreateMapper();
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        public async Task<PagedResults<Question>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            CancellationToken ct)
        {
            return await _questionService.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
        }

        public async Task<Question> FindAsync(Guid id, CancellationToken ct)
        {
            return await _questionService.FindAsync(id, ct);
        }

        public async Task<Question> UpdateAsync(QuestionForm question, Guid userId)
        {
            var questionModel = _mapper.Map<Question>(question);

            var newAnswer = questionModel.Answers.Where(x => !x.Id.HasValue).ToList();
            var existingAnswers = questionModel.Answers.Where(x => x.Id.HasValue).ToList();

            foreach (var answer in newAnswer)
            {
                answer.Id = Guid.NewGuid();
                answer.Active = true;
                answer.CreatedAt = DateTimeOffset.UtcNow;
                answer.ModifiedAt = DateTimeOffset.UtcNow;
                existingAnswers.Remove(answer);
            }

            questionModel.Answers = existingAnswers;

            return await _questionService.UpdateAsync(questionModel, newAnswer, userId);
        }

        public async Task<Guid> AddAsync(QuestionForm question, Guid userId)
        {
            var questionModel = _mapper.Map<Question>(question);

            questionModel.Answers.All(answer =>
            {
                answer.Id = Guid.NewGuid();
                answer.Active = true;
                answer.CreatedAt = DateTimeOffset.UtcNow;
                answer.ModifiedAt = DateTimeOffset.UtcNow;
                return true;
            });

            return await _questionService.AddAsync(questionModel, userId);
        }

        public async Task<Question> RemoveAsync(Guid id)
        {
            return await _questionService.RemoveAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _questionService.ExistsAsync(id);
        }
    }

    public interface IQuestionBusiness
    {
        Task<PagedResults<Question>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            CancellationToken ct);

        Task<Question> FindAsync(Guid id, CancellationToken ct);

        Task<Question> UpdateAsync(QuestionForm question, Guid userId);

        Task<Guid> AddAsync(QuestionForm question, Guid userId);

        Task<Question> RemoveAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
