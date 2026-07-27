using lightClinic.Patient.Contracts.DTOs.Notification;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Notification;

public interface INotificationService
{
    Task<IResult<PaginatedResult<NotificationDto>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IResult<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default);
    Task<IResult<bool>> MarkAsReadAsync(long[] ids, CancellationToken cancellationToken = default);
}
