using lightClinic.Patient.Domain.Common.Interfaces;

namespace lightClinic.Patient.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity, ISoftDelete
    {
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}