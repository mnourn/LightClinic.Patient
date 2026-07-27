namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record VerifyAccountRequestDto
{
    public Guid UserId { get; set; }
    public int OtpId { get; set; }
    public int Otp { get; set; }
}
