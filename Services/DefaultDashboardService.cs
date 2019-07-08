using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultDashboardService : IDashboardService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly UserManager<UserEntity> _userManager;

        public DefaultDashboardService(
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

        public async Task<Event[]> GetStatsAsync(long id, CancellationToken ct)
        {
            IQueryable<EventEntity> query = id == 0 ? _context.Events.AsNoTracking() : _context.Events.AsNoTracking().Where(x => x.Poc.Any(p => p.PocId.Equals(id)));

            var items = await query
                .Include(x => x.Participant)
                .ProjectTo<Event>(_mappingConfiguration)
                .ToArrayAsync(ct);

            return items;
        }
    }
}
