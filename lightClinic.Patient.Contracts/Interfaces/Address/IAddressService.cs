using lightClinic.Patient.Contracts.DTOs.Address;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Address;

public interface IAddressService
{
    Task<IResult<AddressDto>> GetAsync(long id, CancellationToken cancellationToken);
    Task<IResult<List<AddressDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<IResult<AddressDto>> CreateAsync(CreateAddressDto addressDto, CancellationToken cancellationToken);
    Task<IResult<AddressDto>> UpdateAsync(UpdateAddressDto addressDto, CancellationToken cancellationToken);
    Task<IResult<bool>> DeleteAsync(long id, CancellationToken cancellationToken);
}