namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record LoginDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
