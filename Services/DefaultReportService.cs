using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Services
{
    public class DefaultReportService : IReportService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly UserManager<UserEntity> _userManager;

        public DefaultReportService(
            FmsDbContext context,
            IMemoryCache cache,
            IConfigurationProvider mappingConfiguration,
            UserManager<UserEntity> userManager)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _userManager = userManager;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public void FeedbackReportByEvent(Guid id)
        {
            //var eventInfo = await _context.Events.SingleOrDefaultAsync(e => e.Id.Equals(id));

            //eventInfo.p

            //var feedbacks = _context.Feedbacks.Where(f => f.Event.Id.Equals(id));



        }
    }
}
