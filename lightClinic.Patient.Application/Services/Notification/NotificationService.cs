using lightClinic.Patient.Contracts.DTOs.Notification;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Notification;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lightClinic.Patient.Application.Services.Notification;

public class NotificationService : INotificationService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public NotificationService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<IResult<PaginatedResult<NotificationDto>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.GetUserId();
        if (userId == Guid.Empty)
            return await Result<PaginatedResult<NotificationDto>>.FailureAsync("Unauthorized", System.Net.HttpStatusCode.Unauthorized);

        var query = _unitOfWork.Repository<Domain.Entities.Notification>().Entities
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new NotificationDto
            {
                Id = x.Id,
                Title = x.Title,
                Body = x.Body,
                Data = x.Data,
                ReadTime = x.ReadTime,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var paged = PaginatedResult<NotificationDto>.Create(items, totalCount, pageNumber, pageSize);
        return await Result<PaginatedResult<NotificationDto>>.SuccessAsync(paged);
    }

    public async Task<IResult<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.GetUserId();
        if (userId == Guid.Empty)
            return await Result<int>.FailureAsync("Unauthorized", System.Net.HttpStatusCode.Unauthorized);

        var count = await _unitOfWork.Repository<Domain.Entities.Notification>().Entities
            .CountAsync(x => x.UserId == userId && x.ReadTime == null, cancellationToken);

        return await Result<int>.SuccessAsync(count);
    }

    public async Task<IResult<bool>> MarkAsReadAsync(long[] ids, CancellationToken cancellationToken = default)
    {
        if (ids is null || ids.Length == 0 || ids.Any(x => x <= 0))
            return await Result<bool>.FailureAsync("Empty or invalid Ids", System.Net.HttpStatusCode.BadRequest);

        var userId = _currentUser.GetUserId();
        var notifications = await _unitOfWork.Repository<Domain.Entities.Notification>().Entities
            .Where(x => x.UserId == userId && ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Egypt Standard Time");
        foreach (var n in notifications)
        {
            if (n.ReadTime != null)
                return await Result<bool>.FailureAsync($"Notification Id {n.Id} is already readed.", System.Net.HttpStatusCode.UnprocessableEntity);
            n.ReadTime = now;
            await _unitOfWork.Repository<Domain.Entities.Notification>().UpdateAsync(n);
        }

        await _unitOfWork.SaveAsync(cancellationToken);
        return await Result<bool>.SuccessAsync(true, "Marked as read");
    }
}
