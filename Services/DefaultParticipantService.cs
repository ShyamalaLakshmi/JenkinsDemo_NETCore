using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultParticipantService : IParticipantService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultParticipantService(FmsDbContext context, IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _mapper = _mappingConfiguration.CreateMapper();
        }

        public async Task<Participant> FindAsync(Guid id, CancellationToken ct)
        {
            var dbParticipant = await _context.Participants.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, ct);
            if (dbParticipant == null) return null;

            var participant = _mapper.Map<Participant>(dbParticipant);
            return participant;
        }

        public async Task<Participant> UpdateAsync(Participant participant)
        {
            var dbParticipant = _mapper.Map<ParticipantEntity>(participant);
            dbParticipant.ModifiedAt = DateTime.Now;

            _context.Participants.Update(dbParticipant);

            var updated = await _context.SaveChangesAsync();
            if (updated < 1) throw new InvalidOperationException("Could not update participant.");

            return participant;
        }
    }
}
