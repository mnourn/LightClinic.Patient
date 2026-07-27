namespace lightClinic.Patient.Domain.Common.Interfaces
{
    public interface IAuditableEntity
    {
        Guid? CreatedBy { get; set; }
        DateTime? CreatedDate { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        Guid? DeletedBy { get; set; }
    }
}