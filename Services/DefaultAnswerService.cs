using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DefaultAnswerService : IAnswerService
    {
        private readonly FmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly UserManager<UserEntity> _userManager;

        public DefaultAnswerService(
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

        public async Task<Answer> FindAsync(Guid id, CancellationToken ct)
        {
            var dbAnswer = await _context.Answers.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, ct);
            if (dbAnswer == null) return null;

            var answer = _mapper.Map<Answer>(dbAnswer);
            return answer;
        }
    }
}
