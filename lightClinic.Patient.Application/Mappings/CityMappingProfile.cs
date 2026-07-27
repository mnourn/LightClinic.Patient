using lightClinic.Patient.Contracts.DTOs.City;
using lightClinic.Patient.Domain.Entities;

namespace lightClinic.Patient.Application.Mappings;

public class CityMappingProfile : AutoMapper.Profile
{
    public CityMappingProfile()
    {
        CreateMap<City, CityDto>();
    }
}
