using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultFeedbackService : IFeedbackService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly UserManager<UserEntity> _userManager;

        public DefaultFeedbackService(
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

        public async Task<PagedResults<Question>> GetAllByFeedbackTypeAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            string feedbackType,
            CancellationToken ct)
        {
            IQueryable<QuestionEntity> query = _context.Questions.AsNoTracking().Where(x => x.Active && x.FeedbackType.Equals(feedbackType));
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

        public async Task<Guid> AddAsync(Feedback feedback, Guid eventId)
        {
            //var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            //if (user == null) throw new InvalidOperationException("You must be logged in.");

            var dbFeedback = _mapper.Map<FeedbackEntity>(feedback);
            dbFeedback.CreatedAt = DateTime.Now;
            dbFeedback.ModifiedAt = DateTime.Now;

            var dbEvent = await _context.Events.SingleOrDefaultAsync(e => e.Id.Equals(eventId));

            dbFeedback.Event = dbEvent;

            await _context.Feedbacks.AddAsync(dbFeedback);
            var created = await _context.SaveChangesAsync();

            if (created < 1) throw new InvalidOperationException("Could not create feedback.");
            return _mapper.Map<Feedback>(dbFeedback).Id;
        }
    }
}
