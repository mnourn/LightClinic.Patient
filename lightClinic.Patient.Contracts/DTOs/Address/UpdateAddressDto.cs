namespace lightClinic.Patient.Contracts.DTOs.Address;

public record UpdateAddressDto : CreateAddressDto
{
    public long Id { get; set; }
}