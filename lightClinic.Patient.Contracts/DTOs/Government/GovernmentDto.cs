using lightClinic.Patient.Contracts.DTOs.City;

namespace lightClinic.Patient.Contracts.DTOs.Government;

public record GovernmentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public CityDto[] Cities { get; set; }
}
