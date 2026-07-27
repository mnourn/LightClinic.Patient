using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.FamilyMember;

public record CreateFamilyMemberDto
{
    public string FullName { get; set; }
    public string ProfileImage { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string Address { get; set; }
    public Gender? Gender { get; set; }
    public string PhoneNumber { get; set; }
    public FamilyMembership? FamilyMembership { get; set; }
} 