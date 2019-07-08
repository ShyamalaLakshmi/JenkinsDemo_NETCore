using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entities
{
    public class FmsDbContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid>
    {
        public FmsDbContext(DbContextOptions options)
         : base(options) { }

        public DbSet<QuestionEntity> Questions { get; set; }
        public DbSet<AnswerEntity> Answers { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<PocEntity> Pocs { get; set; }
        public DbSet<ParticipantEntity> Participants { get; set; }
        public DbSet<FeedbackEntity> Feedbacks { get; set; }
    }
}
