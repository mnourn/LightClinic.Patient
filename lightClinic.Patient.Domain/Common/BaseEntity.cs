using lightClinic.Patient.Domain.Common.Interfaces;

namespace lightClinic.Patient.Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        public long Id { get; set; }
    }
}