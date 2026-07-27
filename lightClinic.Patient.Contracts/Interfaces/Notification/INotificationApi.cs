using lightClinic.Patient.Contracts.DTOs.Notification;
using lightClinic.Patient.Shared;
using Refit;

namespace lightClinic.Patient.Contracts.Interfaces.Notification;

[Headers(ApplicationConstants.NotificationApiRequestHeader)]
public interface INotificationApi
{
    [Post("/email/create")]
    Task<string> CreateEmailAsync(EmailRequestDto requestDto, CancellationToken cancellationToken);

    [Post("/sms/create")]
    Task<string> CreateSmsAsync(SmsRequestDto requestDto, CancellationToken cancellationToken);
}
