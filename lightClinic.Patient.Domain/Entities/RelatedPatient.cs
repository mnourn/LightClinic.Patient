using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities
{
    public class RelatedPatient : BaseAuditableEntity
    {
        public Guid DoctorId { get; set; }
        public Guid ProfileId { get; set; }

        public Profile Profile { get; set; }
    }
}
