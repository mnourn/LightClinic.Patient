using lightClinic.Patient.Contracts.DTOs.Setting;
using lightClinic.Patient.Shared.Interfaces;

namespace lightClinic.Patient.Contracts.Interfaces.Setting;

public interface ISettingService
{
    Task<IResult<SettingDto>> GetAsync(CancellationToken cancellationToken);

    Task<IResult<SettingDto>> UpdateAsync(UpdateSettingDto setting, CancellationToken cancellationToken);
}
