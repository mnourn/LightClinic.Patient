using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Card;

namespace lightClinic.Patient.Application.Mappings;

public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        CreateMap<Domain.Entities.Card, CardDto>();

        CreateMap<CreateCardDto, Domain.Entities.Card>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<UpdateFamilyMemberDto, Domain.Entities.Card>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
