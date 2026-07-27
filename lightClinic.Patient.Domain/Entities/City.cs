using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities
{
    public class City : BaseAuditableEntity
    {
        public new int Id { get; set; }
        public int GovernmentId { get; set; }
        public string Name { get; set; }

        public Government Government { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}