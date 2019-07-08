using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IAnswerService
    {
        Task<Answer> FindAsync(Guid id, CancellationToken ct);
    }
}
