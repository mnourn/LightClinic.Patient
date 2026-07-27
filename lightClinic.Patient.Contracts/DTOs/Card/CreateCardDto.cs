namespace lightClinic.Patient.Contracts.DTOs.Card;

public record CreateCardDto
{
    public long CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int CVVCode { get; set; }
} 