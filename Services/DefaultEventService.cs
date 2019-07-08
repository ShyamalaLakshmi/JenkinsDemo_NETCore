using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultEventService : IEventService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultEventService(FmsDbContext context, IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public async Task<PagedResults<Event>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            IQueryable<EventEntity> query = _context.Events.AsNoTracking();
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync(ct);

            var items = await query
                .Include(x => x.Poc)
                .Include(x => x.Participant)
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<Event>(_mappingConfiguration)
                .ToArrayAsync(ct);

            return new PagedResults<Event>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<PagedResults<Event>> GetAllByByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct)
        {
            IQueryable<EventEntity> query = _context.Events.AsNoTracking().Where(x => x.Poc.Any(p => p.PocId.Equals(pocId)));
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync(ct);

            var items = await query
                .Include(x => x.Poc)
                .Include(x => x.Participant)
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<Event>(_mappingConfiguration)
                .ToArrayAsync(ct);

            return new PagedResults<Event>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<Event> FindAsync(Guid id, CancellationToken ct)
        {
            var dbEvent = await _context.Events.AsNoTracking().Include(c => c.Poc).Include(c => c.Participant).Include(c => c.Feedback).SingleOrDefaultAsync(c => c.Id == id, ct);
            if (dbEvent == null) return null;

            var eventDetail = _mapper.Map<Event>(dbEvent);
            return eventDetail;
        }

        public async Task<bool> AddAsync(List<Event> eventSummaryDetails)
        {
            var dbEvents = _mapper.Map<List<EventEntity>>(eventSummaryDetails);
            dbEvents.All(x =>
            {
                x.CreatedAt = DateTime.Now;
                x.ModifiedAt = DateTime.Now;
                return true;
            });

            await _context.Events.AddRangeAsync(dbEvents);
            var created = await _context.SaveChangesAsync();
            return created > 1;
        }
    }
}
