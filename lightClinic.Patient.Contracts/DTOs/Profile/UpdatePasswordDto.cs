namespace lightClinic.Patient.Contracts.DTOs.Profile;

public record UpdatePasswordDto
{
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}
