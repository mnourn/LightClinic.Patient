using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Setting;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Setting;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lightClinic.Patient.Application.Services.Setting;

public class SettingService : ISettingService, IApplicationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateSettingDto> _updateValidator;

    public SettingService(
        UserManager<ApplicationUser> userManager,
        ICurrentUser currentUser,
        IMapper mapper,
        IValidator<UpdateSettingDto> updateValidator)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _mapper = mapper;
        _updateValidator = updateValidator;
    }

    public async Task<IResult<SettingDto>> GetAsync(CancellationToken cancellationToken)
    {
        var setting = await _userManager.Users
            .Where(x => x.Id == _currentUser.GetUserId())
            .ProjectTo<SettingDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (setting is null)
        {
            return await Result<SettingDto>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<SettingDto>.SuccessAsync(setting);
    }

    public async Task<IResult<SettingDto>> UpdateAsync(UpdateSettingDto setting, CancellationToken cancellationToken)
    {
        var validationResult = _updateValidator.Validate(setting);
        if (!validationResult.IsValid)
        {
            return await Result<SettingDto>.ValidationFailureAsync(validationResult.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _currentUser.GetUserId(), cancellationToken);

        if (user is null)
        {
            return await Result<SettingDto>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }
        bool updated = false;
        if (setting.Language is not null && user.Language != setting.Language.Value)
        {
            user.Language = setting.Language.Value;
            updated = true;
        }
        if (setting.NotificationEnabled is not null && user.NotificationEnabled != setting.NotificationEnabled.Value)
        {
            user.NotificationEnabled = setting.NotificationEnabled.Value;
            updated = true;
        }
        if (!updated)
        {
            return await Result<SettingDto>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }
        var res = await _userManager.UpdateAsync(user);
        if (!res.Succeeded)
        {
            return await Result<SettingDto>.FailureAsync(res.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<SettingDto>.SuccessAsync(_mapper.Map<SettingDto>(user), "Setting updated successfully");
    }
}
