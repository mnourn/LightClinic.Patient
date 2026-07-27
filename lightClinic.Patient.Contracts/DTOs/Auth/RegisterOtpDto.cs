namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record RegisterOtpDto
{
    public Guid UserId { get; set; }
    public long OtpId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
