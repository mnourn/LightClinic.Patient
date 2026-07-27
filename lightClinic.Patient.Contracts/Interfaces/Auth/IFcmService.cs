using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Auth;

public interface IFcmService
{
    Task<IResult<bool>> UpdateAsync(string fcmToken, string deviceId, CancellationToken cancellationToken);
}
