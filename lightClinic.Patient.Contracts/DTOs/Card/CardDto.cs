namespace lightClinic.Patient.Contracts.DTOs.Card;

public record CardDto
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public long CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int CVVCode { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModificationTime { get; set; }
} 