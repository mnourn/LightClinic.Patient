using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.Government;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Government;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Application.Services.Government;

public class GovernmentService(IGovernmentRepository governmentRepository, IMapper mapper) : IGovernmentService, IApplicationService
{
    private readonly IGovernmentRepository _governmentRepository = governmentRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult<List<GovernmentDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _governmentRepository.GetAllAsync(cancellationToken);

        return await Result<List<GovernmentDto>>.SuccessAsync(_mapper.Map<List<GovernmentDto>>(entities));
    }
}
