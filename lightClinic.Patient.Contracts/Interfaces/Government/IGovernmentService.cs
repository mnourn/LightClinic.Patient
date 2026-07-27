using lightClinic.Patient.Contracts.DTOs.Government;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Government;

public interface IGovernmentService
{
    Task<IResult<List<GovernmentDto>>> GetAllAsync(CancellationToken cancellationToken);
}