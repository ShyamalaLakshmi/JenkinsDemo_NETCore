using AutoMapper;
using Entities;
using Models;

namespace Api.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<QuestionEntity, Question>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.QuestionsController.GetQuestionById),
                        null)));

            CreateMap<QuestionForm, Question>();

            CreateMap<Question, QuestionEntity>()
                .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<AnswerForm, Answer>();

            CreateMap<Answer, AnswerEntity>();

            CreateMap<EventEntity, Event>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.EventsController.GetEventById),
                        null)));

            CreateMap<Poc, PocEntity>();

            CreateMap<Participant, ParticipantEntity>();

            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.UsersController.GetUserById),
                        new { userId = src.Id })));

            CreateMap<Feedback, FeedbackEntity>();

            CreateMap<Event, EventReport>();

            CreateMap<Event, EventStatus>();

            CreateMap<Event, EventStatusReport>();
        }
    }
}
