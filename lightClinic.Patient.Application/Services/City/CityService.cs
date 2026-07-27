using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.City;
using lightClinic.Patient.Contracts.Interfaces.City;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Application.Services.City;

public class CityService(IUnitOfWork unitOfWork, IMapper mapper) : ICityService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult<List<CityDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.Repository<Domain.Entities.City>().GetAllAsync(cancellationToken);

        return await Result<List<CityDto>>.SuccessAsync(_mapper.Map<List<CityDto>>(entities));
    }
}
