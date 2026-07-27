namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record VerifyResetPasswordOtpDto
{
    public Guid UserId { get; set; }
    public bool Verified { get; set; }
    public string ResetPasswordToken { get; set; }
}
