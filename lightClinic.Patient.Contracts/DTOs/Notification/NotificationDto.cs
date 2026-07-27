namespace lightClinic.Patient.Contracts.DTOs.Notification;

public class NotificationDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Data { get; set; }
    public DateTime? ReadTime { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool IsRead => ReadTime.HasValue;
}
