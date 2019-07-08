using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultQuestionService : IQuestionService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly UserManager<UserEntity> _userManager;

        public DefaultQuestionService(
            FmsDbContext context,
            IMemoryCache cache,
            IConfigurationProvider mappingConfiguration,
            UserManager<UserEntity> userManager)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _userManager = userManager;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public async Task<PagedResults<Question>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            CancellationToken ct)
        {
            IQueryable<QuestionEntity> query = _context.Questions.AsNoTracking().Where(x => x.Active);
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync(ct);

            var items = await query
                .Include(x => x.Answers)
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                //.ProjectTo<Question>(_mappingConfiguration)
                .Select(x => new Question
                {
                    Id = x.Id,
                    Description = x.Description,
                    Answers = x.Answers.Where(a => a.Active).AsQueryable().ProjectTo<Answer>(_mappingConfiguration),
                    Active = x.Active,
                    AllowMultipleAnswer = x.AllowMultipleAnswer,
                    FreeTextQuestion = x.FreeTextQuestion,
                    CustomQuestion = x.CustomQuestion,
                    FeedbackType = x.FeedbackType,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt
                })
                .ToArrayAsync(ct);

            return new PagedResults<Question>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<Question> FindAsync(Guid id, CancellationToken ct)
        {
            var dbQuestion = await _context.Questions.AsNoTracking().Include(c => c.Answers).SingleOrDefaultAsync(c => c.Id == id, ct);
            if (dbQuestion == null) return null;

            var question = _mapper.Map<Question>(dbQuestion);
            return question;
        }

        public async Task<Question> UpdateAsync(Question question, IEnumerable<Answer> newAnswer, Guid userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new InvalidOperationException("You must be logged in.");

            var dbQuestion = _mapper.Map<QuestionEntity>(question);
            dbQuestion.ModifiedAt = DateTimeOffset.UtcNow;
            dbQuestion.User = user;

            _context.Questions.Update(dbQuestion);

            if (newAnswer.Any())
            {
                var dbAnswers = _mapper.Map<IEnumerable<AnswerEntity>>(newAnswer);
                dbAnswers.All(answerEntity =>
                {
                    answerEntity.Question = dbQuestion;
                    answerEntity.User = user;
                    return true;
                });

                _context.Answers.AddRange(dbAnswers);
            }

            var updated = await _context.SaveChangesAsync();
            if (updated < 1) throw new InvalidOperationException("Could not update question.");

            return question;
        }

        public async Task<Guid> AddAsync(Question question, Guid userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new InvalidOperationException("You must be logged in.");

            var dbQuestion = _mapper.Map<QuestionEntity>(question);
            dbQuestion.Active = true;
            dbQuestion.CreatedAt = DateTimeOffset.UtcNow;
            dbQuestion.ModifiedAt = DateTimeOffset.UtcNow;
            dbQuestion.User = user;

            await _context.Questions.AddAsync(dbQuestion);
            var created = await _context.SaveChangesAsync();

            if (created < 1) throw new InvalidOperationException("Could not create question.");
            return _mapper.Map<Question>(dbQuestion).Id;
        }

        public async Task<Question> RemoveAsync(Guid id)
        {
            var dbQuestion = await _context.Questions.FindAsync(id);
            dbQuestion.Active = false;

            _context.Questions.Update(dbQuestion);
            await _context.SaveChangesAsync();
            return _mapper.Map<Question>(dbQuestion);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Questions.AsNoTracking().AnyAsync(c => c.Id == id);
        }
    }
}
