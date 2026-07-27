using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record LoginResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public Language Language { get; set; }
    public bool NotificationEnabled { get; set; }
}
