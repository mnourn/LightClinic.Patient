namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record UserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
}
