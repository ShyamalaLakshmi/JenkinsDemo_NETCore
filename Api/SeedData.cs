using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await AddTestUsers(
                services.GetRequiredService<RoleManager<UserRoleEntity>>(),
                services.GetRequiredService<UserManager<UserEntity>>());

            await AddTestData(
                services.GetRequiredService<FmsDbContext>(),
                services.GetRequiredService<UserManager<UserEntity>>());
        }

        public static async Task AddTestData(
            FmsDbContext context,
            UserManager<UserEntity> userManager)
        {
            if (context.Questions.Any())
            {
                // Already has data
                return;
            }

            var adminUser = userManager.Users
                .SingleOrDefault(u => u.Email == "admin@outreach.com");

            context.Questions.Add(new QuestionEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195976"),
                Description = "How do you rate the overall event?",
                Answers = new[]
                {
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9995971"),
                        Description = "1",
                        Icon = "far fa-angry fa-5x text-red",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195981"),
                        Description = "2",
                        Icon = "far fa-frown fa-5x text-yellow-darker",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195972"),
                        Description = "3",
                        Icon = "far fa-meh fa-5x text-warning",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195961"),
                        Description = "4",
                        Icon = "far fa-smile fa-5x text-lime",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195971"),
                        Description = "5",
                        Icon = "far fa-grin-hearts fa-5x text-green",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                },
                User = adminUser,
                Active = true,
                AllowMultipleAnswer = false,
                FreeTextQuestion = false,
                CustomQuestion = true,
                FeedbackType = "participated",
                CreatedAt = DateTimeOffset.UtcNow
            });

            context.Questions.Add(new QuestionEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195974"),
                Description = "What did you like about this volunteering activity?",
                User = adminUser,
                Active = true,
                AllowMultipleAnswer = false,
                FreeTextQuestion = true,
                CustomQuestion = false,
                FeedbackType = "participated",
                CreatedAt = DateTimeOffset.UtcNow
            });

            context.Questions.Add(new QuestionEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195975"),
                Description = "What can be improved in this volunteering activity?",
                User = adminUser,
                Active = true,
                AllowMultipleAnswer = false,
                FreeTextQuestion = true,
                CustomQuestion = false,
                FeedbackType = "participated",
                CreatedAt = DateTimeOffset.UtcNow
            });

            context.Questions.Add(new QuestionEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195973"),
                Description = "Hey there, You had registered for the event on saturday. We would like to know the reason for not joining the event to understand if the team which created the event has some room for improvement in their process, so that we get 100% participation from the registered attendees.",
                Answers = new[]
                {
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-25a9e9995971"),
                        Description = "Unexpected Personal Committment",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-26a9e9995971"),
                        Description = "Unexpected Official Work",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-27a9e9995971"),
                        Description = "Even Not What I Expected",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-28a9e9995971"),
                        Description = "Did Not Receive Further Information About The Event",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-29a9e9995971"),
                        Description = "Incorrectly Registered",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-24a9e9995971"),
                        Description = "Do Not Wish to Disclose",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                },
                User = adminUser,
                Active = true,
                AllowMultipleAnswer = false,
                FreeTextQuestion = false,
                CustomQuestion = false,
                FeedbackType = "notparticipated",
                CreatedAt = DateTimeOffset.UtcNow
            });

            context.Questions.Add(new QuestionEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9c9195975"),
                Description = "Hey there, Please share your feedback for unregistering from the event ?",
                Answers = new[]
                {
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-45a9e9995971"),
                        Description = "Unexpected Personal Committment",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-46a9e9995971"),
                        Description = "Unexpected Official Work",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-47a9e9995971"),
                        Description = "Even Not What I Expected",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-48a9e9995971"),
                        Description = "Did Not Receive Further Information About The Event",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-49a9e9995971"),
                        Description = "Incorrectly Registered",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new AnswerEntity
                    {
                        Id = Guid.Parse("ee2b83be-91db-4de5-8122-50a9e9995971"),
                        Description = "Do Not Wish to Disclose",
                        Active = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                },
                User = adminUser,
                Active = true,
                AllowMultipleAnswer = false,
                FreeTextQuestion = false,
                CustomQuestion = false,
                FeedbackType = "unregistered",
                CreatedAt = DateTimeOffset.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task AddTestUsers(
            RoleManager<UserRoleEntity> roleManager,
            UserManager<UserEntity> userManager)
        {
            var dataExists = roleManager.Roles.Any() || userManager.Users.Any();
            if (dataExists)
            {
                return;
            }

            // Add a test role
            await roleManager.CreateAsync(new UserRoleEntity("Admin"));
            await roleManager.CreateAsync(new UserRoleEntity("Pmo"));
            await roleManager.CreateAsync(new UserRoleEntity("Poc"));

            // Add a test user
            var user = new UserEntity
            {
                Email = "admin@outreach.com",
                UserName = "admin@outreach.com",
                FirstName = "Admin",
                LastName = "Tester",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(user, "Admin@123");

            // Put the user in the admin role
            await userManager.AddToRoleAsync(user, "Admin");
            await userManager.UpdateAsync(user);

            var pmo = new UserEntity
            {
                Email = "123456@cognizant.com",
                UserName = "123456@cognizant.com",
                FirstName = "Pmo",
                LastName = "Tester",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(pmo, "Pmo@123");

            // Put the user in the admin role
            await userManager.AddToRoleAsync(pmo, "Pmo");
            await userManager.UpdateAsync(pmo);
        }
    }
}
