using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Http;

namespace lightClinic.Patient.Contracts.Interfaces.Profile;

public interface IProfileService
{
    Task<IResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken);
    Task<IResult<ProfileDto>> CreateAsync(CreateUserDto user, CancellationToken cancellationToken);
    Task<IResult<ProfileDto>> GetAsync(CancellationToken cancellationToken);
    Task<IResult<bool>> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto, CancellationToken cancellationToken);
    Task<IResult<List<ProfileDto>>> GetProfilesAsync(Guid[] ids, CancellationToken cancellationToken);
    Task<IResult<ProfileDto>> UpdateAsync(UpdateProfileDto profile, CancellationToken cancellationToken);
    Task<IResult<FamilyMemberDto>> GetFamilyMemberAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult<List<FamilyMemberDto>>> GetAllFamilyMembersAsync(CancellationToken cancellationToken);
    Task<IResult<FamilyMemberDto>> CreateFamilyMemberAsync(CreateFamilyMemberDto memberDto, CancellationToken cancellationToken);
    Task<IResult<FamilyMemberDto>> UpdateFamilyMemberAsync(UpdateFamilyMemberDto memberDto, CancellationToken cancellationToken);
    Task<IResult<bool>> DeleteFamilyMemberAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult<List<ProfileDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
    Task<IResult<ProfileDto>> GetByProfileAsync(Guid profileId, CancellationToken cancellationToken);
    Task<IResult<List<ProfileLocationDto>>> GetByIdsAsync(Guid[] ids, CancellationToken cancellationToken);
}
