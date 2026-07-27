using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities;

public class Card : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public long CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int CVVCode { get; set; }

    public ApplicationUser User { get; set; }
} 