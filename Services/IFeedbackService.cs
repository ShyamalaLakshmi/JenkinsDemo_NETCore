using Entities;
using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IFeedbackService
    {
        Task<PagedResults<Question>> GetAllByFeedbackTypeAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            string feedbackType,
            CancellationToken ct);

        Task<Guid> AddAsync(Feedback feedback, Guid eventId);
    }
}
