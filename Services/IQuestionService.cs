using Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IQuestionService
    {
        Task<PagedResults<Question>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            CancellationToken ct);

        Task<Question> FindAsync(Guid id, CancellationToken ct);

        Task<Question> UpdateAsync(Question category, IEnumerable<Answer> newAnswer ,Guid userId);

        Task<Guid> AddAsync(Question category, Guid userId);

        Task<Question> RemoveAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
