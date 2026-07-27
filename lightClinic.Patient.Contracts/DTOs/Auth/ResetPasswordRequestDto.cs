namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record ResetPasswordRequestDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
