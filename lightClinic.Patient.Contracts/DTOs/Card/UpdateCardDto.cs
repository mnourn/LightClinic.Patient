namespace lightClinic.Patient.Contracts.DTOs.Card;

public record UpdateCardDto : CreateCardDto
{
    public long Id { get; set; }
}