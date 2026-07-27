using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.Setting;

namespace lightClinic.Patient.Application.Mappings;

public class SettingMappingProfile : Profile
{
    public SettingMappingProfile()
    {
        CreateMap<Domain.Entities.ApplicationUser, SettingDto>();
    }
}
