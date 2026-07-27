namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record OtpRequestDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
