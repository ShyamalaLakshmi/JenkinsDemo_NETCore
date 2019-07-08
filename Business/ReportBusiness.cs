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
    public class ReportBusiness : IReportBusiness
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public ReportBusiness(IEventService eventService, IConfigurationProvider mappingConfiguration)
        {
            _eventService = eventService;
            _mapper = mappingConfiguration.CreateMapper();
        }

        public async Task<PagedResults<EventStatus>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            var events = await _eventService.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
            var eventReport = new List<EventStatus>();

            foreach (var eventInfo in events.Items)
            {
                var eventStatus = _mapper.Map<EventStatus>(eventInfo);
                eventStatus.TotalParticipants = eventInfo.Participant.Count();
                eventStatus.TotalAttended = eventInfo.Participant.Count(p => p.Attended);
                eventStatus.TotalUnregistered = eventInfo.Participant.Count(p => p.Unregistered);
                eventStatus.TotalNotAttended = eventInfo.Participant.Count(p => p.NotAttended);
                eventStatus.TotalFeedbackReceived = eventInfo.Feedback.Count();
                eventStatus.TotalAttendedFeedback = eventInfo.Participant.Count(p => p.Attended && p.IsFeedbackReceived);
                eventStatus.TotalUnregisteredFeedback = eventInfo.Participant.Count(p => p.Unregistered && p.IsFeedbackReceived);
                eventStatus.TotalNotAttendedFeedback = eventInfo.Participant.Count(p => p.NotAttended && p.IsFeedbackReceived);

                var fiveRating = eventInfo.Feedback.Count(x => x.Rating.Equals(5));
                var fourRating = eventInfo.Feedback.Count(x => x.Rating.Equals(4));
                var threeRating = eventInfo.Feedback.Count(x => x.Rating.Equals(3));
                var twoRating = eventInfo.Feedback.Count(x => x.Rating.Equals(2));
                var oneRating = eventInfo.Feedback.Count(x => x.Rating.Equals(1));
                var totalCount = fiveRating + fourRating + threeRating + twoRating + oneRating;

                if (totalCount != 0)
                {
                    var averageRating = (decimal)((fiveRating * 5) + (fourRating * 4) + (threeRating * 3) + (twoRating * 2) + oneRating) / totalCount;
                    eventStatus.AverageRating = averageRating;
                }

                eventReport.Add(eventStatus);
            }

            var pagedResult = new PagedResults<EventStatus>
            {
                Items = eventReport,
                TotalSize = events.TotalSize
            };

            return pagedResult;
        }

        public async Task<PagedResults<EventStatus>> GetAllByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct)
        {
            var events = await _eventService.GetAllByByPocAsync(pagingOptions, sortOptions, searchOptions, pocId, ct);
            var eventReport = new List<EventStatus>();

            foreach (var eventInfo in events.Items)
            {
                var eventStatus = _mapper.Map<EventStatus>(eventInfo);
                eventStatus.TotalParticipants = eventInfo.Participant.Count();
                eventStatus.TotalAttended = eventInfo.Participant.Count(p => p.Attended);
                eventStatus.TotalUnregistered = eventInfo.Participant.Count(p => p.Unregistered);
                eventStatus.TotalNotAttended = eventInfo.Participant.Count(p => p.NotAttended);
                eventStatus.TotalFeedbackReceived = eventInfo.Feedback.Count();
                eventStatus.TotalAttendedFeedback = eventInfo.Participant.Count(p => p.Attended && p.IsFeedbackReceived);
                eventStatus.TotalUnregisteredFeedback = eventInfo.Participant.Count(p => p.Unregistered && p.IsFeedbackReceived);
                eventStatus.TotalNotAttendedFeedback = eventInfo.Participant.Count(p => p.NotAttended && p.IsFeedbackReceived);

                var fiveRating = eventInfo.Feedback.Count(x => x.Rating.Equals(5));
                var fourRating = eventInfo.Feedback.Count(x => x.Rating.Equals(4));
                var threeRating = eventInfo.Feedback.Count(x => x.Rating.Equals(3));
                var twoRating = eventInfo.Feedback.Count(x => x.Rating.Equals(2));
                var oneRating = eventInfo.Feedback.Count(x => x.Rating.Equals(1));
                var totalCount = fiveRating + fourRating + threeRating + twoRating + oneRating;

                if (totalCount != 0)
                {
                    var averageRating = (decimal)((fiveRating * 5) + (fourRating * 4) + (threeRating * 3) + (twoRating * 2) + oneRating) / totalCount;
                    eventStatus.AverageRating = averageRating;
                }

                eventReport.Add(eventStatus);
            }

            var pagedResult = new PagedResults<EventStatus>
            {
                Items = eventReport,
                TotalSize = events.TotalSize
            };

            return pagedResult;
        }

        public async Task<PagedResults<EventStatusReport>> GetAllReportAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct)
        {
            var events = await _eventService.GetAllAsync(pagingOptions, sortOptions, searchOptions, ct);
            var eventReport = new List<EventStatusReport>();

            foreach (var eventInfo in events.Items)
            {
                var eventStatus = _mapper.Map<EventStatusReport>(eventInfo);
                eventStatus.TotalParticipants = eventInfo.Participant.Count();
                eventStatus.TotalAttended = eventInfo.Participant.Count(p => p.Attended);
                eventStatus.TotalUnregistered = eventInfo.Participant.Count(p => p.Unregistered);
                eventStatus.TotalNotAttended = eventInfo.Participant.Count(p => p.NotAttended);
                eventStatus.TotalFeedbackReceived = eventInfo.Feedback.Count();
                eventStatus.TotalAttendedFeedback = eventInfo.Participant.Count(p => p.Attended && p.IsFeedbackReceived);
                eventStatus.TotalUnregisteredFeedback = eventInfo.Participant.Count(p => p.Unregistered && p.IsFeedbackReceived);
                eventStatus.TotalNotAttendedFeedback = eventInfo.Participant.Count(p => p.NotAttended && p.IsFeedbackReceived);

                var fiveRating = eventInfo.Feedback.Count(x => x.Rating.Equals(5));
                var fourRating = eventInfo.Feedback.Count(x => x.Rating.Equals(4));
                var threeRating = eventInfo.Feedback.Count(x => x.Rating.Equals(3));
                var twoRating = eventInfo.Feedback.Count(x => x.Rating.Equals(2));
                var oneRating = eventInfo.Feedback.Count(x => x.Rating.Equals(1));
                var totalCount = fiveRating + fourRating + threeRating + twoRating + oneRating;

                if (totalCount != 0)
                {
                    var averageRating = (decimal)((fiveRating * 5) + (fourRating * 4) + (threeRating * 3) + (twoRating * 2) + oneRating) / totalCount;
                    eventStatus.AverageRating = averageRating;
                }

                eventReport.Add(eventStatus);
            }

            var pagedResult = new PagedResults<EventStatusReport>
            {
                Items = eventReport,
                TotalSize = events.TotalSize
            };

            return pagedResult;
        }

        public async Task<PagedResults<EventStatusReport>> GetAllReportByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct)
        {
            var events = await _eventService.GetAllByByPocAsync(pagingOptions, sortOptions, searchOptions, pocId, ct);
            var eventReport = new List<EventStatusReport>();

            foreach (var eventInfo in events.Items)
            {
                var eventStatus = _mapper.Map<EventStatusReport>(eventInfo);
                eventStatus.TotalParticipants = eventInfo.Participant.Count();
                eventStatus.TotalAttended = eventInfo.Participant.Count(p => p.Attended);
                eventStatus.TotalUnregistered = eventInfo.Participant.Count(p => p.Unregistered);
                eventStatus.TotalNotAttended = eventInfo.Participant.Count(p => p.NotAttended);
                eventStatus.TotalFeedbackReceived = eventInfo.Feedback.Count();
                eventStatus.TotalAttendedFeedback = eventInfo.Participant.Count(p => p.Attended && p.IsFeedbackReceived);
                eventStatus.TotalUnregisteredFeedback = eventInfo.Participant.Count(p => p.Unregistered && p.IsFeedbackReceived);
                eventStatus.TotalNotAttendedFeedback = eventInfo.Participant.Count(p => p.NotAttended && p.IsFeedbackReceived);

                var fiveRating = eventInfo.Feedback.Count(x => x.Rating.Equals(5));
                var fourRating = eventInfo.Feedback.Count(x => x.Rating.Equals(4));
                var threeRating = eventInfo.Feedback.Count(x => x.Rating.Equals(3));
                var twoRating = eventInfo.Feedback.Count(x => x.Rating.Equals(2));
                var oneRating = eventInfo.Feedback.Count(x => x.Rating.Equals(1));
                var totalCount = fiveRating + fourRating + threeRating + twoRating + oneRating;

                if (totalCount != 0)
                {
                    var averageRating = (decimal)((fiveRating * 5) + (fourRating * 4) + (threeRating * 3) + (twoRating * 2) + oneRating) / totalCount;
                    eventStatus.AverageRating = averageRating;
                }

                eventReport.Add(eventStatus);
            }

            var pagedResult = new PagedResults<EventStatusReport>
            {
                Items = eventReport,
                TotalSize = events.TotalSize
            };

            return pagedResult;
        }

        public async void GetCompleteEventReport(Guid eventId)
        {
            var eventInfo = await _eventService.FindAsync(eventId, CancellationToken.None);
            var eventReport = _mapper.Map<EventStatus>(eventInfo);

            eventReport.TotalParticipants = eventInfo.Participant.Count();
            eventReport.TotalAttended = eventInfo.Participant.Count(p => p.Attended);
            eventReport.TotalUnregistered = eventInfo.Participant.Count(p => p.Unregistered);
            eventReport.TotalNotAttended = eventInfo.Participant.Count(p => p.NotAttended);
            eventReport.TotalAttendedFeedback = eventInfo.Participant.Count(p => p.Attended && p.IsFeedbackReceived);
            eventReport.TotalUnregisteredFeedback = eventInfo.Participant.Count(p => p.Unregistered && p.IsFeedbackReceived);
            eventReport.TotalNotAttendedFeedback = eventInfo.Participant.Count(p => p.NotAttended && p.IsFeedbackReceived);

        }
    }

    public interface IReportBusiness
    {
        Task<PagedResults<EventStatus>> GetAllAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct);

        Task<PagedResults<EventStatus>> GetAllByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct);

        Task<PagedResults<EventStatusReport>> GetAllReportAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            CancellationToken ct);

        Task<PagedResults<EventStatusReport>> GetAllReportByPocAsync(
            PagingOptions pagingOptions,
            SortOptions<Event, EventEntity> sortOptions,
            SearchOptions<Event, EventEntity> searchOptions,
            long pocId,
            CancellationToken ct);

        void GetCompleteEventReport(Guid eventId);
    }
}
