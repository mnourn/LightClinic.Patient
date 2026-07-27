using lightClinic.Patient.Contracts.Interfaces.Notification;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class NotificationEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.PatientRoleName))
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        group.MapGet("/GetPaged", GetPagedAsync)
            .WithName("GetNotificationsPaged")
            .WithSummary("Get notifications (paged)")
            .WithDescription("Returns a paged list of notifications for the current user");

        group.MapGet("/GetUnreadCount", GetUnreadCountAsync)
            .WithName("GetUnreadNotificationCount")
            .WithSummary("Get unread count")
            .WithDescription("Returns the count of unread notifications for the current user");

        group.MapPost("/MarkAsRead", MarkAsReadAsync)
            .WithName("MarkNotificationsAsRead")
            .WithSummary("Mark as read")
            .WithDescription("Marks the given notifications as read");
    }

    private static async Task<IResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await notificationService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Results.Json(result, statusCode: (int)(result.Code ?? System.Net.HttpStatusCode.OK));
    }

    private static async Task<IResult> GetUnreadCountAsync(
        INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        var result = await notificationService.GetUnreadCountAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)(result.Code ?? System.Net.HttpStatusCode.OK));
    }

    private static async Task<IResult> MarkAsReadAsync(
        long[] ids,
        INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        var result = await notificationService.MarkAsReadAsync(ids, cancellationToken);
        return Results.Json(result, statusCode: (int)(result.Code ?? System.Net.HttpStatusCode.OK));
    }
}
