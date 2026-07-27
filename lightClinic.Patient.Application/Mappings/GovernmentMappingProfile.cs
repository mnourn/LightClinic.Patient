using lightClinic.Patient.Contracts.DTOs.Government;
using lightClinic.Patient.Domain.Entities;

namespace lightClinic.Patient.Application.Mappings;

public class GovernmentMappingProfile : AutoMapper.Profile
{
    public GovernmentMappingProfile()
    {
        CreateMap<Government, GovernmentDto>();
    }
}
