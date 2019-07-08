using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IDashboardService
    {
        Task<Event[]> GetStatsAsync(long id, CancellationToken ct);
    }
}
