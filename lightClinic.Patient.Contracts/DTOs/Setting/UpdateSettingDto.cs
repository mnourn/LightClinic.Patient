using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Setting;

public record UpdateSettingDto
{
    public Language? Language { get; set; }
    public bool? NotificationEnabled { get; set; }
}
