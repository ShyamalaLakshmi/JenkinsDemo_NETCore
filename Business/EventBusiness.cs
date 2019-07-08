using AutoMapper;
using Entities;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business
{
    public class EventBusiness : IEventBusiness
    {
        private readonly IEventService _eventService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;

        public EventBusiness(IEventService eventService,
            IEmailService emailService,
            IConfigurationProvider mappingConfiguration)
        {
            _eventService = eventService;
            _emailService = emailService;
            _mappingConfiguration = mappingConfiguration;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public async Task<PagedResults<Event>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            return await _eventService.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
        }

        public async Task<PagedResults<Event>> GetAllByByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct)
        {
            return await _eventService.GetAllByByPocAsync(pagingOptions, sortOptions, searchOptions, pocId, ct);
        }

        public async Task<Event> FindAsync(Guid id, CancellationToken ct)
        {
            var eventInfo = await _eventService.FindAsync(id, ct);

            var fiveRating = eventInfo.Feedback.Count(x => x.Rating.Equals(5));
            var fourRating = eventInfo.Feedback.Count(x => x.Rating.Equals(4));
            var threeRating = eventInfo.Feedback.Count(x => x.Rating.Equals(3));
            var twoRating = eventInfo.Feedback.Count(x => x.Rating.Equals(2));
            var oneRating = eventInfo.Feedback.Count(x => x.Rating.Equals(1));
            var totalCount = fiveRating + fourRating + threeRating + twoRating + oneRating;

            if (totalCount != 0)
            {
                var averageRating = (decimal)((fiveRating * 5) + (fourRating * 4) + (threeRating * 3) + (twoRating * 2) + oneRating) / totalCount;
                eventInfo.AverageRating = averageRating;
            }


            return eventInfo;
        }

        public async Task<bool> AddAsync(List<Event> eventSummaryDetails)
        {
            return await _eventService.AddAsync(eventSummaryDetails);
        }

        public async Task<PagedResults<EventReport>> GetAllForExcelAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct)
        {
            var events = pocId == 0
                    ? await GetAllAsync(pagingOptions, sortOptions, searchOptions, ct)
                    : await GetAllByByPocAsync(pagingOptions, sortOptions, searchOptions, pocId, ct);

            var report = _mapper.Map<PagedResults<EventReport>>(events);

            return report;
        }

        public async Task<Event> CreateReminderEmailAsync(Guid eventId)
        {
            return await FindAsync(eventId, CancellationToken.None);
        }
    }

    public interface IEventBusiness
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

        Task<PagedResults<EventReport>> GetAllForExcelAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct);

        Task<Event> CreateReminderEmailAsync(Guid eventId);
    }
}
