using Models;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business
{
    public class ParticipantBusiness
    {
        private readonly IParticipantService _participantService;

        public ParticipantBusiness(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public async Task<Participant> FindAsync(Guid id, CancellationToken ct)
        {
            return await _participantService.FindAsync(id, ct);
        }
    }
}
