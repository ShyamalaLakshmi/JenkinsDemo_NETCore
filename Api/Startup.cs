using Api.Filters;
using Api.Infrastructure;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Business;
using Entities;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using NSwag.AspNetCore;
using OpenIddict.Validation;
using Services;
using System;

namespace Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.Configure<PagingOptions>(
                Configuration.GetSection("DefaultPagingOptions"));

            services.AddScoped<IQuestionService, DefaultQuestionService>();
            services.AddScoped<IUserService, DefaultUserService>();
            services.AddScoped<IFeedbackService, DefaultFeedbackService>();
            services.AddScoped<IEventService, DefaultEventService>();
            services.AddScoped<IEmailService, DemoEmailService>();
            services.AddScoped<IAnswerService, DefaultAnswerService>();
            services.AddScoped<IParticipantService, DefaultParticipantService>();
            services.AddScoped<IDashboardService, DefaultDashboardService>();
            services.AddScoped<IReportService, DefaultReportService>();
            services.AddScoped<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            services.AddScoped<IExcelService, ExcelService>();

            services.AddScoped<IQuestionBusiness, QuestionBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IFeedbackBusiness, FeedbackBusiness>();
            services.AddScoped<IEventBusiness, EventBusiness>();
            services.AddScoped<AnswerBusiness, AnswerBusiness>();
            services.AddScoped<ParticipantBusiness, ParticipantBusiness>();
            services.AddScoped<IDashboardBusiness, DashboardBusiness>();
            services.AddScoped<IReportBusiness, ReportBusiness>();

            // Use in-memory database for quick dev and testing
            // TODO: Swap out for a real database in production
            services.AddDbContext<FmsDbContext>(
                options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    options.UseOpenIddict<Guid>();
                });

            // Add OpenIddict services
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<FmsDbContext>()
                        .ReplaceDefaultEntities<Guid>();
                })
                .AddServer(options =>
                {
                    options.UseMvc();

                    options.EnableTokenEndpoint("/api/token");

                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();

                    // Disabled for Development purpose
                    options.DisableHttpsRequirement();
                })
                .AddValidation();

            // ASP.NET Core Identity should use the same claim names as OpenIddict
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationDefaults.AuthenticationScheme;
            });

            // Add Hangfire services.  
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));

            // Add ASP.NET Core Identity
            AddIdentityCoreServices(services);

            services
                .AddResponseCompression()
                .AddMvc(options =>
                {
                    options.CacheProfiles.Add("Static", new CacheProfile { Duration = 86400 });
                    options.CacheProfiles.Add("Collection", new CacheProfile { Duration = 60 });
                    options.CacheProfiles.Add("Resource", new CacheProfile { Duration = 180 });

                    options.Filters.Add<JsonExceptionFilter>();
                    //options.Filters.Add<RequireHttpsOrCloseAttribute>();
                    options.Filters.Add<LinkRewritingFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    // These should be the defaults, but we can be explicit:
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                });

            services.AddSwaggerDocument();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader
                    = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector
                    = new CurrentImplementationApiVersionSelector(options);
            });

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFms", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiError(context.ModelState);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddResponseCaching();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewAllUsersPolicy",
                    p => p.RequireAuthenticatedUser().RequireRole("Admin"));
                options.AddPolicy("ViewAllEventsPolicy",
                    p => p.RequireAuthenticatedUser().RequireRole("Admin", "Pmo"));
                options.AddPolicy("ViewFeedbackQuestionsPolicy",
                    p => p.RequireAuthenticatedUser().RequireRole("Admin"));
                options.AddPolicy("ViewRolesPolicy",
                    p => p.RequireAuthenticatedUser().RequireRole("Admin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUi3();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!env.IsEnvironment("Testing"))
            {
                //Hangfire
                app.UseHangfireDashboard();
                app.UseHangfireServer();

                //BackgroundJob.Enqueue<IExcelService>((excel) => excel.GetEventSummaryInformation());
                RecurringJob.AddOrUpdate<IExcelService>((excel) => excel.GetEventSummaryInformation(), Cron.Hourly);
            }

            app.UseCors("AllowFms");
            app.UseAuthentication();
            app.UseResponseCaching();
            app.UseResponseCompression();
            app.UseMvc();
        }

        private static void AddIdentityCoreServices(IServiceCollection services)
        {
            var builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(
                builder.UserType,
                typeof(UserRoleEntity),
                builder.Services);

            builder.AddRoles<UserRoleEntity>()
                .AddEntityFrameworkStores<FmsDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<UserEntity>>();
        }
    }
}
