using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Profile;

public record ProfileLocationDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string ProfileImageUrl { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string Address { get; set; }
    public Gender? Gender { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? CityId { get; set; }
    public int? GovernmentId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
