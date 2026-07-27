namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record VerifyOtpRequestDto
{
    public long OtpId { get; set; }
    public int Otp { get; set; }
    public Guid UserId { get; set; }
}
