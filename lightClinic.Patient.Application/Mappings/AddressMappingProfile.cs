using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Address;

namespace lightClinic.Patient.Application.Mappings;

public class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<Domain.Entities.Address, AddressDto>();

        CreateMap<CreateAddressDto, Domain.Entities.Address>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<UpdateFamilyMemberDto, Domain.Entities.Address>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
