using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record RegisterUserDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
