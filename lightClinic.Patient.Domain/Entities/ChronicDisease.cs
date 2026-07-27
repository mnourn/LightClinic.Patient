using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities
{
    public class ChronicDisease : BaseAuditableEntity
    {
        public new int Id { get; set; }
        public string Name { get; set; }
    }
}
