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
    public class FeedbackBusiness : IFeedbackBusiness
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IUserService _userService;
        private readonly IQuestionService _questionService;
        private readonly IEmailService _emailService;
        private readonly IAnswerService _answerService;
        private readonly IParticipantService _participantService;
        private readonly IMapper _mapper;
        private readonly PagingOptions _defaultPagingOptions;

        public FeedbackBusiness(
            IFeedbackService feedbackService,
            IUserService userService,
            IQuestionService questionService,
            IEmailService emailService,
            IAnswerService answerService,
            IParticipantService participantService,
            IConfigurationProvider mappingConfiguration,
            IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _feedbackService = feedbackService;
            _userService = userService;
            _questionService = questionService;
            _emailService = emailService;
            _answerService = answerService;
            _participantService = participantService;
            _mapper = mappingConfiguration.CreateMapper();
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        public async Task<PagedResults<Question>> GetAllByFeedbackTypeAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            string feedbackType,
            CancellationToken ct)
        {
            return await _feedbackService.GetAllByFeedbackTypeAsync(pagingOptions, sortOptions, searchOptions, feedbackType, ct);
        }

        public async Task AddAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            FeedbackForm feedback)
        {
            var questions = await _feedbackService.GetAllByFeedbackTypeAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedback.FeedbackType, CancellationToken.None);

            var feedbackModel = new Feedback();

            foreach (var reason in feedback.Reason)
            {
                var question = questions.Items.Single(x => x.Id.Equals(reason.QuestionId));

                feedbackModel.Attended = question.FeedbackType.Equals("participated");
                feedbackModel.NotAttended = question.FeedbackType.Equals("notparticipated");
                feedbackModel.Unregistered = question.FeedbackType.Equals("unregistered");

                if (question.CustomQuestion)
                {
                    var dbAnswer = question.Answers.Single(a => a.Id.Equals(Guid.Parse(reason.AnswerId)));
                    feedbackModel.ReasonId = Guid.Parse(reason.AnswerId);
                    feedbackModel.Rating = Convert.ToInt32(dbAnswer.Description);
                }

                if (!question.FreeTextQuestion)
                {
                    var dbAnswer = question.Answers.Single(a => a.Id.Equals(Guid.Parse(reason.AnswerId)));
                    feedbackModel.ReasonId = Guid.Parse(reason.AnswerId);
                    feedbackModel.Cons = dbAnswer.Description;
                }

                if (question.FreeTextQuestion && question.Description.Contains("like"))
                    feedbackModel.Pros = reason.AnswerId;
                if (question.FreeTextQuestion && question.Description.Contains("improved"))
                    feedbackModel.Cons = reason.AnswerId;
            }

            var created = await _feedbackService.AddAsync(feedbackModel, feedback.EventId);

            var participant = await _participantService.FindAsync(feedback.ParticipantId, CancellationToken.None);

            await _emailService.SendAsync("outreachadmin@cognizant.com", "Admin", participant.EmployeeId, "Feedback Received", "Thanks for the feedback.");

            participant.FeedbackId = created;
            participant.IsFeedbackReceived = true;

            await _participantService.UpdateAsync(participant);

        }

    }
    public interface IFeedbackBusiness
    {
        Task<PagedResults<Question>> GetAllByFeedbackTypeAsync(
              PagingOptions pagingOptions,
              SortOptions<Question, QuestionEntity> sortOptions,
              SearchOptions<Question, QuestionEntity> searchOptions,
              string feedbackType,
              CancellationToken ct);

        Task AddAsync(
            PagingOptions pagingOptions,
            SortOptions<Question, QuestionEntity> sortOptions,
            SearchOptions<Question, QuestionEntity> searchOptions,
            FeedbackForm feedback);

    }
}
