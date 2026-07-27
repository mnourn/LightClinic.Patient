namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record ForgetPasswordRequestDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
