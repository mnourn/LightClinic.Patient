namespace lightClinic.Patient.Contracts.DTOs.City;

public record CityDto
{
    public int Id { get; set; }
    public int GovernmentId { get; set; }
    public string Name { get; set; }
}
