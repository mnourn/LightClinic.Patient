using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Profile;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Contracts.Interfaces;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace lightClinic.Patient.Application.Services.Profile;

public class ProfileService : IProfileService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<UpdateProfileDto> _updateValidator;
    private readonly IValidator<UpdatePasswordDto> _updatePasswordValidator;
    private readonly IValidator<CreateFamilyMemberDto> _createFamilyMemberValidator;
    private readonly IValidator<UpdateFamilyMemberDto> _updateFamilyMemberValidator;

    public ProfileService(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMediaService mediaService,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<UpdateProfileDto> updateValidator,
        IValidator<UpdatePasswordDto> updatePasswordValidator,
        IValidator<CreateFamilyMemberDto> createFamilyMemberValidator,
        IValidator<UpdateFamilyMemberDto> updateFamilyMemberValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _mediaService = mediaService;
        _userManager = userManager;
        _createUserValidator = createUserValidator;
        _updateValidator = updateValidator;
        _updatePasswordValidator = updatePasswordValidator;
        _createFamilyMemberValidator = createFamilyMemberValidator;
        _updateFamilyMemberValidator = updateFamilyMemberValidator;
    }

    public async Task<IResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var filename = await _mediaService.SaveAsync(file);

        return filename.Succeed ? Result<string>.Success(filename.Message, "File uploaded successfully") : Result<string>.Failure(filename.Message, System.Net.HttpStatusCode.BadRequest);
    }

    public async Task<IResult<ProfileDto>> CreateAsync(CreateUserDto user, CancellationToken cancellationToken)
    {
        var resultValidation = _createUserValidator.Validate(user);
        if (!resultValidation.IsValid)
        {
            return await Result<ProfileDto>.ValidationFailureAsync(resultValidation.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }
        if (!string.IsNullOrWhiteSpace(user.Email) && await _userManager.Users
            .AnyAsync(x => x.Email.Trim().ToLower() == user.Email.Trim().ToLower(), cancellationToken))
        {
            return await Result<ProfileDto>.FailureAsync("Email is already existed!", System.Net.HttpStatusCode.BadRequest);
        }
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await _userManager.Users
            .AnyAsync(x => x.PhoneNumber.Trim().ToLower() == user.PhoneNumber.Trim().ToLower(), cancellationToken))
        {
            return await Result<ProfileDto>.FailureAsync("PhoneNumber is already existed!", System.Net.HttpStatusCode.BadRequest);
        }

        var profile = new Domain.Entities.Profile()
        {
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Gender = user.Gender,
            Address = user.Address,
            RelatedPatients = [new RelatedPatient() { DoctorId = _currentUser.GetUserId() }],
        };
        var newUser = new ApplicationUser
        {
            UserName = user.Email,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CreatedDate = DateTime.UtcNow,
            Profiles = [profile],
        };

        var res = await _userManager.CreateAsync(newUser);
        if (!res.Succeeded)
        {
            return await Result<ProfileDto>.FailureAsync(res.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
        }
        var result = _mapper.Map<ProfileDto>(newUser.Profiles.First());
        _mapper.Map(newUser, result);

        return await Result<ProfileDto>.SuccessAsync(result, "User created successfully");
    }

    public async Task<IResult<ProfileDto>> GetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(x => x.Profiles.Where(y => y.FamilyMembership == null))
            .FirstOrDefaultAsync(x => x.Id == _currentUser.GetUserId(), cancellationToken);

        if (user is null)
        {
            return await Result<ProfileDto>.FailureAsync("Profile not found!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<ProfileDto>.SuccessAsync(MapToProfileDto(user));
    }

    public async Task<IResult<bool>> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto, CancellationToken cancellationToken)
    {
        var validationResult = _updatePasswordValidator.Validate(updatePasswordDto);
        if (!validationResult.IsValid)
        {
            return await Result<bool>.ValidationFailureAsync(validationResult.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _currentUser.GetUserId(), cancellationToken);
        if (!await _userManager.CheckPasswordAsync(user, updatePasswordDto.OldPassword))
        {
            return await Result<bool>.FailureAsync("Incorrect old Password!", System.Net.HttpStatusCode.BadRequest);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, updatePasswordDto.NewPassword);

        return result.Succeeded ?
            await Result<bool>.SuccessAsync("Password updated successfully") :
            await Result<bool>.FailureAsync(result.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
    }

    public async Task<IResult<List<ProfileDto>>> GetProfilesAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        if (ids is null || ids.Length == 0)
        {
            return await Result<List<ProfileDto>>.FailureAsync("Empty list!", System.Net.HttpStatusCode.BadRequest);
        }

        var profiles = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .Where(x => ids.Contains(x.Id))
            .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var item in profiles.Where(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl)))
        {
            item.ProfileImageUrl = _mediaService.GetBlobUrl(item.ProfileImageUrl);
        }

        return await Result<List<ProfileDto>>.SuccessAsync(profiles);
    }

    public async Task<IResult<ProfileDto>> UpdateAsync(UpdateProfileDto profile, CancellationToken cancellationToken)
    {
        var result = _updateValidator.Validate(profile);
        if (!result.IsValid)
        {
            return await Result<ProfileDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var user = await _userManager.Users
            .Include(x => x.Profiles.Where(y => y.FamilyMembership == null))
            .FirstOrDefaultAsync(x => x.Id == _currentUser.GetUserId(), cancellationToken);

        if (user is null)
        {
            return await Result<ProfileDto>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }
        var infoUpdated = await UpdateEmailAndPhoneNumberAsync(user, profile);
        if (!infoUpdated.Result)
        {
            return await Result<ProfileDto>.FailureAsync(infoUpdated.Message, System.Net.HttpStatusCode.BadRequest);
        }
        var updated = UpdateProfile(user.Profiles.FirstOrDefault(x => x.FamilyMembership == null), profile);
        if (profile.ProfileImage is not null)
        {
            var upload = await _mediaService.ConfirmAsync(profile.ProfileImage);
            if (!upload.Succeed)
            {
                return await Result<ProfileDto>.FailureAsync(upload.Result, System.Net.HttpStatusCode.BadRequest);
            }
            user.Profiles.FirstOrDefault(x => x.FamilyMembership == null).ProfileImage = upload.Result;
            updated = true;
        }
        if (!updated)
        {
            return await Result<ProfileDto>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }
        var res = await _userManager.UpdateAsync(user);

        return res.Succeeded ?
            await Result<ProfileDto>.SuccessAsync(MapToProfileDto(user), "Patient profile updated successfully") :
            await Result<ProfileDto>.FailureAsync(res.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
    }

    public async Task<IResult<FamilyMemberDto>> GetFamilyMemberAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .ProjectTo<FamilyMemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id && x.FamilyMembership != null, cancellationToken);

        if (member is null)
        {
            return await Result<FamilyMemberDto>.FailureAsync("FamilyMember not found!", System.Net.HttpStatusCode.BadRequest);
        }
        if (member.UserId != _currentUser.GetUserId())
        {
            return await Result<FamilyMemberDto>.FailureAsync("FamilyMember does not belong to you!", System.Net.HttpStatusCode.BadRequest);
        }
        member.ProfileImageUrl = _mediaService.GetBlobUrl(member.ProfileImageUrl);

        return await Result<FamilyMemberDto>.SuccessAsync(member);
    }

    public async Task<IResult<List<FamilyMemberDto>>> GetAllFamilyMembersAsync(CancellationToken cancellationToken)
    {
        var members = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .Where(x => x.UserId == _currentUser.GetUserId())
            .ProjectTo<FamilyMemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var item in members)
        {
            item.ProfileImageUrl = _mediaService.GetBlobUrl(item.ProfileImageUrl);
        }

        return await Result<List<FamilyMemberDto>>.SuccessAsync(members);
    }

    public async Task<IResult<FamilyMemberDto>> CreateFamilyMemberAsync(CreateFamilyMemberDto memberDto, CancellationToken cancellationToken)
    {
        var result = _createFamilyMemberValidator.Validate(memberDto);
        if (!result.IsValid)
        {
            return await Result<FamilyMemberDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var validations = await CreateMemberValidationAsync(memberDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<FamilyMemberDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }
        var upload = await _mediaService.ConfirmAsync(memberDto.ProfileImage);
        if (!upload.Succeed)
        {
            return await Result<FamilyMemberDto>.FailureAsync(upload.Result, System.Net.HttpStatusCode.BadRequest);
        }

        var member = _mapper.Map<Domain.Entities.Profile>(memberDto);
        member.UserId = _currentUser.GetUserId();
        member.ProfileImage = upload.Result;

        await _unitOfWork.Repository<Domain.Entities.Profile>().AddAsync(member);

        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<FamilyMemberDto>.FailureAsync("Connection failed!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<FamilyMemberDto>.SuccessAsync(_mapper.Map<FamilyMemberDto>(member), "FamilyMember created successfully");
    }

    public async Task<IResult<FamilyMemberDto>> UpdateFamilyMemberAsync(UpdateFamilyMemberDto memberDto, CancellationToken cancellationToken)
    {
        var result = _updateFamilyMemberValidator.Validate(memberDto);
        if (!result.IsValid)
        {
            return await Result<FamilyMemberDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var member = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .FirstOrDefaultAsync(x => x.Id == memberDto.Id && x.FamilyMembership != null, cancellationToken);

        if (member is null)
        {
            return await Result<FamilyMemberDto>.FailureAsync("FamilyMember not found!", System.Net.HttpStatusCode.BadRequest);
        }

        var validations = await UpdateMemberValidationAsync(member, memberDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<FamilyMemberDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }
        _mapper.Map(memberDto, member);
        if (memberDto.ProfileImage is not null)
        {
            var upload = await _mediaService.ConfirmAsync(memberDto.ProfileImage);
            if (!upload.Succeed)
            {
                return await Result<FamilyMemberDto>.FailureAsync(upload.Result, System.Net.HttpStatusCode.BadRequest);
            }
            member.ProfileImage = upload.Result;
        }

        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<FamilyMemberDto>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<FamilyMemberDto>.SuccessAsync(_mapper.Map<FamilyMemberDto>(member), "FamilyMember updated successfully");
    }

    public async Task<IResult<bool>> DeleteFamilyMemberAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .FirstOrDefaultAsync(x => x.Id == id && x.FamilyMembership != null, cancellationToken);

        if (member is null)
        {
            return await Result<bool>.FailureAsync("FamilyMember not found!", System.Net.HttpStatusCode.BadRequest);
        }
        if (member.UserId != _currentUser.GetUserId())
        {
            return await Result<bool>.FailureAsync("FamilyMember does not belong to you!", System.Net.HttpStatusCode.BadRequest);
        }

        await _unitOfWork.Repository<Domain.Entities.Profile>().DeleteAsync(member);
        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<bool>.FailureAsync("Connection failed!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<bool>.SuccessAsync("FamilyMember deleted successfully");
    }

    public async Task<IResult<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Result<bool>.SuccessAsync(await _userManager.Users.AnyAsync(x => x.Id == id));
    }

    public async Task<IResult<List<ProfileDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await Result<List<ProfileDto>>.FailureAsync("Search term cannot be empty!", System.Net.HttpStatusCode.BadRequest);
        }
        if (searchTerm.Length < ApplicationConstants.MinSearchTextLength)
        {
            return await Result<List<ProfileDto>>.FailureAsync($"Search term length must be at least {ApplicationConstants.MinSearchTextLength} characters!", System.Net.HttpStatusCode.BadRequest);
        }
        string loweredTerm = searchTerm.Trim().ToLower();
        var profiles = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .Where(x => x.Id.ToString().Contains(loweredTerm) || x.PhoneNumber.Trim().ToLower().Contains(loweredTerm))
            .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var item in profiles.Where(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl)))
        {
            item.ProfileImageUrl = _mediaService.GetBlobUrl(item.ProfileImageUrl);
        }

        return await Result<List<ProfileDto>>.SuccessAsync(profiles);
    }

    public async Task<IResult<ProfileDto>> GetByProfileAsync(Guid profileId, CancellationToken cancellationToken)
    {
        if (profileId == Guid.Empty)
        {
            return await Result<ProfileDto>.FailureAsync("Invalid Profile Id!", System.Net.HttpStatusCode.BadRequest);
        }

        var profile = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .Include(x => x.User)
            .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == profileId, cancellationToken);

        if (profile is null)
        {
            return await Result<ProfileDto>.FailureAsync("Profile not found!", System.Net.HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(profile.ProfileImageUrl))
        {
            profile.ProfileImageUrl = _mediaService.GetBlobUrl(profile.ProfileImageUrl);
        }

        return await Result<ProfileDto>.SuccessAsync(profile);
    }

    public async Task<IResult<List<ProfileLocationDto>>> GetByIdsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        if (ids is null || ids.Length == 0)
        {
            return await Result<List<ProfileLocationDto>>.FailureAsync("Empty Ids!", System.Net.HttpStatusCode.BadRequest);
        }

        var profiles = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
            .Include(x => x.User)
            .Where(x => ids.Contains(x.UserId) && x.FamilyMembership == null)
            .ProjectTo<ProfileLocationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (profiles is null || profiles.Count == 0)
        {
            return await Result<List<ProfileLocationDto>>.SuccessAsync([], "Profiles not found!");
        }
        foreach (var profile in profiles.Where(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl)))
        {
            profile.ProfileImageUrl = _mediaService.GetBlobUrl(profile.ProfileImageUrl);
        }
        var addresses = await _unitOfWork.Repository<Domain.Entities.Address>().Entities
            .Include(x => x.City)
            .Where(x => ids.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                g.OrderBy(x => x.LocationType).First().CityId,
                g.OrderBy(x => x.LocationType).First().City.GovernmentId,
                g.OrderBy(x => x.LocationType).First().Latitude,
                g.OrderBy(x => x.LocationType).First().Longitude
            })
            .ToListAsync(cancellationToken);

        foreach (var addr in addresses)
        {
            var profile = profiles.FirstOrDefault(x => x.Id == addr.UserId);
            if (profile is null)
                continue;
            profile.CityId = addr.CityId;
            profile.GovernmentId = addr.GovernmentId;
            profile.Latitude = addr.Latitude;
            profile.Longitude = addr.Longitude;
        }

        return await Result<List<ProfileLocationDto>>.SuccessAsync(profiles);
    }

    private async Task<(bool Result, string Message)> UpdateEmailAndPhoneNumberAsync(ApplicationUser user, UpdateProfileDto profile)
    {
        var emailUpdated = await UpdateEmailAsync(user, profile);
        if (!emailUpdated.Result && emailUpdated.Message is not null)
        {
            return (false, emailUpdated.Message);
        }
        var phoneUpdated = await UpdatePhoneNumberAsync(user, profile);
        if (!phoneUpdated.Result && phoneUpdated.Message is not null)
        {
            return (false, phoneUpdated.Message);
        }

        return (true, null);
    }

    private async Task<(bool Result, string Message)> UpdateEmailAsync(ApplicationUser user, UpdateProfileDto profile)
    {
        if (!string.IsNullOrWhiteSpace(profile.Email) && !string.Equals(user.Email, profile.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userManager.Users.AnyAsync(x => x.Id != user.Id && x.Email.Trim().ToLower() == profile.Email.Trim().ToLower()))
            {
                return (false, "Email exists!");
            }
            user.Email = profile.Email;

            return (true, null);
        }

        return (false, null);
    }

    private async Task<(bool Result, string Message)> UpdatePhoneNumberAsync(ApplicationUser user, UpdateProfileDto profile)
    {
        if (!string.IsNullOrWhiteSpace(profile.PhoneNumber) && !string.Equals(user.PhoneNumber, profile.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userManager.Users.AnyAsync(x => x.Id != user.Id && x.PhoneNumber.Trim().ToLower() == profile.PhoneNumber.Trim().ToLower()))
            {
                return (false, "PhoneNumber exists!");
            }
            user.PhoneNumber = user.Profiles.FirstOrDefault(x => x.FamilyMembership == null).PhoneNumber = profile.PhoneNumber;

            return (true, null);
        }

        return (false, null);
    }

    private async Task<string> CreateMemberValidationAsync(CreateFamilyMemberDto memberDto, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<Domain.Entities.Profile>().Entities.AnyAsync(x
            => x.UserId == _currentUser.GetUserId()
            && x.FullName.Trim().ToLower() == memberDto.FullName.Trim().ToLower()
            && x.FamilyMembership == memberDto.FamilyMembership, cancellationToken))
        {
            return "Member already exists!";
        }

        return null;
    }

    private async Task<string> UpdateMemberValidationAsync(Domain.Entities.Profile profile, UpdateFamilyMemberDto memberDto, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        if (profile.FamilyMembership is null)
        {
            return "Invalid FamilyMember data!";
        }
        if (profile.UserId != userId)
        {
            return "FamilyMember does not belong to you!";
        }
        if (await _unitOfWork.Repository<Domain.Entities.Profile>().Entities.AnyAsync(x
            => x.Id != memberDto.Id
            && x.UserId == userId
            && x.FullName.Trim().ToLower() == memberDto.FullName.Trim().ToLower()
            && x.FamilyMembership == memberDto.FamilyMembership, cancellationToken))
        {
            return "Member with FamilyMembership & FullName already exists!";
        }

        return null;
    }

    private ProfileDto MapToProfileDto(ApplicationUser entity)
    {
        if (entity is null)
            return null;

        var result = _mapper.Map<ProfileDto>(entity);
        var profile = entity.Profiles?.FirstOrDefault(x => x.FamilyMembership == null);

        if (profile is not null)
        {
            _mapper.Map(profile, result);
            result.ProfileImageUrl = _mediaService.GetBlobUrl(profile.ProfileImage);
        }

        return result;
    }

    private bool UpdateProfile(Domain.Entities.Profile entity, UpdateProfileDto updateUserProfileDto)
    {
        bool res = false;
        Type profileType = typeof(Domain.Entities.Profile);
        foreach (var property in typeof(UpdateProfileDto).GetProperties())
        {
            var prop = profileType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop is null)
                continue;

            var originalValue = prop.GetValue(entity);
            var currentValue = property.GetValue(updateUserProfileDto);

            if (currentValue is not null && !object.Equals(originalValue, currentValue))
            {
                prop.SetValue(entity, currentValue, null);
                res = true;
            }
        }

        return res;
    }
}
