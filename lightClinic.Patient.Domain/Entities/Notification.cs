using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Data { get; set; }
    public DateTime? ReadTime { get; set; }

    public ApplicationUser User { get; set; }
} 