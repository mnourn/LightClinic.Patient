namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record VerifyResetPasswordRequestDto
{
    public int OtpId { get; set; }
    public int Otp { get; set; }
    public Guid UserId { get; set; }
}
