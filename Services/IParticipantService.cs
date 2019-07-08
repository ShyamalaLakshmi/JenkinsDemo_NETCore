using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IParticipantService
    {
        Task<Participant> FindAsync(Guid id, CancellationToken ct);

        Task<Participant> UpdateAsync(Participant participant);
    }
}
