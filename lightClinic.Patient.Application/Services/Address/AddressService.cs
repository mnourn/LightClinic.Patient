using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Address;
using lightClinic.Patient.Contracts.Interfaces.Address;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lightClinic.Patient.Application.Services.Address;

public class AddressService : IAddressService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateAddressDto> _createValidator;
    private readonly IValidator<UpdateAddressDto> _updateValidator;

    public AddressService(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMapper mapper,
        IValidator<CreateAddressDto> createValidator,
        IValidator<UpdateAddressDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IResult<AddressDto>> GetAsync(long id, CancellationToken cancellationToken)
    {
        var address = await _unitOfWork.Repository<Domain.Entities.Address>().Entities
            .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (address is null)
        {
            return await Result<AddressDto>.FailureAsync("Address not found!", System.Net.HttpStatusCode.BadRequest);
        }
        if (address.UserId != _currentUser.GetUserId())
        {
            return await Result<AddressDto>.FailureAsync("Address does not belong to you!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<AddressDto>.SuccessAsync(address);
    }

    public async Task<IResult<List<AddressDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var addresses = await _unitOfWork.Repository<Domain.Entities.Address>().Entities
            .Where(x => x.UserId == _currentUser.GetUserId())
            .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<AddressDto>>.SuccessAsync(addresses);
    }

    public async Task<IResult<AddressDto>> CreateAsync(CreateAddressDto addressDto, CancellationToken cancellationToken)
    {
        var result = _createValidator.Validate(addressDto);
        if (!result.IsValid)
        {
            return await Result<AddressDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }
        var validations = await CreateValidationAsync(addressDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<AddressDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        var address = _mapper.Map<Domain.Entities.Address>(addressDto);
        address.UserId = _currentUser.GetUserId();

        await _unitOfWork.Repository<Domain.Entities.Address>().AddAsync(address);

        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<AddressDto>.FailureAsync("Connection failed!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<AddressDto>.SuccessAsync(_mapper.Map<AddressDto>(address), "Address created successfully");
    }

    public async Task<IResult<AddressDto>> UpdateAsync(UpdateAddressDto addressDto, CancellationToken cancellationToken)
    {
        var result = _updateValidator.Validate(addressDto);
        if (!result.IsValid)
        {
            return await Result<AddressDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var address = await _unitOfWork.Repository<Domain.Entities.Address>().GetByIdAsync(addressDto.Id, cancellationToken);

        var validations = await UpdateValidationAsync(address, addressDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<AddressDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        _mapper.Map(addressDto, address);

        await _unitOfWork.Repository<Domain.Entities.Address>().UpdateAsync(address);
        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<AddressDto>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<AddressDto>.SuccessAsync(_mapper.Map<AddressDto>(address), "Address updated successfully");
    }

    public async Task<IResult<bool>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return await Result<bool>.FailureAsync("Invalid Id", System.Net.HttpStatusCode.BadRequest);
        }
        var address = await _unitOfWork.Repository<Domain.Entities.Address>().GetByIdAsync(id, cancellationToken);

        var validations = DeleteValidation(address);
        if (validations is not null)
        {
            return await Result<bool>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        await _unitOfWork.Repository<Domain.Entities.Address>().DeleteAsync(address);
        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<bool>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<bool>.SuccessAsync("Address deleted successfully");
    }

    private async Task<string> CreateValidationAsync(CreateAddressDto addressDto, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<Domain.Entities.Address>().Entities
            .AnyAsync(x => x.UserId == _currentUser.GetUserId() && x.LocationType == addressDto.LocationType, cancellationToken))
        {
            return "Address with LocationType already exists";
        }

        return null;
    }

    private async Task<string> UpdateValidationAsync(Domain.Entities.Address address, UpdateAddressDto addressDto, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        if (address is null)
        {
            return "Address not found!";
        }
        if (address.UserId != userId)
        {
            return "Address does not belong to you!";
        }
        if (await _unitOfWork.Repository<Domain.Entities.Address>().Entities
            .AnyAsync(x
                => x.Id != addressDto.Id
                && x.UserId == userId
                && x.LocationType == addressDto.LocationType, cancellationToken))
        {
            return "Address with LocationType already exists";
        }

        return null;
    }

    private string DeleteValidation(Domain.Entities.Address address)
    {
        if (address is null)
        {
            return "Address not found!";
        }
        if (address.UserId != _currentUser.GetUserId())
        {
            return "Address does not belong to you!";
        }

        return null;
    }
}