using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Contracts.DTOs.Address;

public record CreateAddressDto
{
    public string Location { get; set; }
    public LocationType LocationType { get; set; }
    public string BuildingName { get; set; }
    public byte AptNo { get; set; }
    public byte Floor { get; set; }
    public string Street { get; set; }
    public string AdditionalDirections { get; set; }
    public string PhoneNumber { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? CityId { get; set; }
}