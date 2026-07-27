using AutoMapper;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Profile;

namespace lightClinic.Patient.Application.Mappings;

public class ProfileMappingProfile : Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<Domain.Entities.ApplicationUser, ProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Domain.Entities.Profile, ProfileDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImage));

        CreateMap<Domain.Entities.Profile, ProfileLocationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImage))
            .ForMember(dest => dest.CityId, opt => opt.Ignore())
            .ForMember(dest => dest.GovernmentId, opt => opt.Ignore())
            .ForMember(dest => dest.Latitude, opt => opt.Ignore())
            .ForMember(dest => dest.Longitude, opt => opt.Ignore());

        CreateMap<Domain.Entities.Profile, FamilyMemberDto>()
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImage));

        CreateMap<CreateFamilyMemberDto, Domain.Entities.Profile>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());

        CreateMap<UpdateFamilyMemberDto, Domain.Entities.Profile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());
    }
}
