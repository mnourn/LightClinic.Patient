namespace lightClinic.Patient.Contracts.DTOs.Notification;

public record SmsRequestDto
{
    public string To { get; set; }
    public string Message { get; set; }
}
