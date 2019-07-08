using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Models;
using Services;

namespace Business
{
    public class DashboardBusiness : IDashboardBusiness
    {
        private readonly IDashboardService _dashboardService;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DashboardBusiness(IDashboardService dashboardService, IConfigurationProvider mappingConfiguration)
        {
            _dashboardService = dashboardService;
            _mappingConfiguration = mappingConfiguration;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public async Task<Dashboard> GetStatsAsync(long id, CancellationToken ct)
        {
            var events = await _dashboardService.GetStatsAsync(id, ct);

            var dashboard = new Dashboard
            {
                TotalEvents = events.Length,
                LivesImpacted = events.Sum(e => e.LivesImpacted),
                TotalVolunteers = events.Sum(e => e.TotalNoOfVolunteers),
                TotalParticipants = events.Sum(e => e.Participant.Count())
            };

            return dashboard;
        }
    }
    public interface IDashboardBusiness
    {
        Task<Dashboard> GetStatsAsync(long id, CancellationToken ct);
    }
}
