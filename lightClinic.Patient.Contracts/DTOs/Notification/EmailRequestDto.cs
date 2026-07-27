namespace lightClinic.Patient.Contracts.DTOs.Notification;

public record EmailRequestDto
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string CC { get; set; }
    public string Body { get; set; }
    //public AttachmentDto[] Attachments { get; set; }
}
