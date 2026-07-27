using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lightClinic.Patient.Application.Services.Auth;

public class FcmService : IFcmService, IApplicationService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public FcmService(ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<bool>> UpdateAsync(string fcmToken, string deviceId, CancellationToken cancellationToken)
    {
        var error = ValidateFcmToken(fcmToken, deviceId);
        if (error is not null)
        {
            return await Result<bool>.FailureAsync(error, System.Net.HttpStatusCode.BadRequest);
        }
        var token = await _unitOfWork.Repository<NotificationToken>().Entities
            .FirstOrDefaultAsync(x => x.UserId == _currentUser.GetUserId() && x.DeviceId == deviceId, cancellationToken);
        if (token is null)
        {
            await _unitOfWork.Repository<NotificationToken>().AddAsync(new NotificationToken()
            {
                UserId = _currentUser.GetUserId(),
                DeviceId = deviceId,
                FCMToken = fcmToken,
            });
        }
        else
        {
            if (token.FCMToken == fcmToken)
            {
                return await Result<bool>.SuccessAsync(true, "FCM Token is already up to date.");
            }
            token.FCMToken = fcmToken;
            token.DeviceId = deviceId;
            await _unitOfWork.Repository<NotificationToken>().UpdateAsync(token);
        }

        await _unitOfWork.SaveAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(true, "FCM Token updated successfully");
    }

    private static string ValidateFcmToken(string fcmToken, string deviceId)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
        {
            return "Empty FCM Token.";
        }
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return "Empty DeviceId.";
        }
        if (fcmToken.Length > ApplicationConstants.MaxFCMTokenLength)
        {
            return $"FCM Token exceeds maximum {ApplicationConstants.MaxFCMTokenLength} length.";
        }
        if (deviceId.Length > ApplicationConstants.MaxFCMTokenLength)
        {
            return $"DeviceId exceeds maximum {ApplicationConstants.MaxFCMTokenLength} length.";
        }

        return null;
    }
}
