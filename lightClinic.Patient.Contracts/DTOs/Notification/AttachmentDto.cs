namespace lightClinic.Patient.Contracts.DTOs.Notification;

public record AttachmentDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string AttachmentName { get; set; }
}
