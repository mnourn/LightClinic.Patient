using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities;

public class NotificationToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string FCMToken { get; set; }
    public string DeviceId { get; set; }

    public ApplicationUser User { get; set; }
} 