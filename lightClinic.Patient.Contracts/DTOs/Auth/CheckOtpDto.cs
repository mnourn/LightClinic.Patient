namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record CheckOtpDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
