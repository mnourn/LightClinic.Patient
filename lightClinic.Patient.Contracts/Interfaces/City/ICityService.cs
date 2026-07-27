using lightClinic.Patient.Contracts.DTOs.City;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.City;

public interface ICityService
{
    Task<IResult<List<CityDto>>> GetAllAsync(CancellationToken cancellationToken);
}