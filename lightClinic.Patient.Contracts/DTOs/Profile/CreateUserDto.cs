using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Profile;

public record CreateUserDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public Gender Gender { get; set; }
}
