using AutoMapper;
using Microsoft.Extensions.Options;
using Models;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business
{
    public class AnswerBusiness
    {
        private readonly IAnswerService _answerService;
        private readonly IMapper _mapper;
        private readonly PagingOptions _defaultPagingOptions;

        public AnswerBusiness(IAnswerService answerService, IConfigurationProvider mappingConfiguration, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _answerService = answerService;
            _mapper = mappingConfiguration.CreateMapper();
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        public async Task<Answer> FindAsync(Guid id, CancellationToken ct)
        {
            return await _answerService.FindAsync(id, ct);
        }
    }
}
