using Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IEventService
    {
        Task<PagedResults<Event>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct);

        Task<PagedResults<Event>> GetAllByByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct);

        Task<Event> FindAsync(Guid id, CancellationToken ct);

        Task<bool> AddAsync(List<Event> eventSummaryDetails);
    }
}
