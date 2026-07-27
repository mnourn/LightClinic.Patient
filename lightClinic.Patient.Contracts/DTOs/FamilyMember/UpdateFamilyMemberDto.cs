namespace lightClinic.Patient.Contracts.DTOs.FamilyMember;

public record UpdateFamilyMemberDto : CreateFamilyMemberDto
{
    public Guid Id { get; set; }
}