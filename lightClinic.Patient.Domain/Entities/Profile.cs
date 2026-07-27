using lightClinic.Patient.Domain.Common;
using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Domain.Entities
{
    public class Profile : BaseAuditableEntity
    {
        public new Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Address { get; set; }
        public Gender? Gender { get; set; }
        public FamilyMembership? FamilyMembership { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<RelatedPatient> RelatedPatients { get; set; }
        public ICollection<Medication> Medications { get; set; }

        public Profile()
        {
            Id = Guid.CreateVersion7();
        }
    }
} 