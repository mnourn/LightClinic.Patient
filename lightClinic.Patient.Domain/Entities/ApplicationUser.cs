using lightClinic.Patient.Domain.Common.Interfaces;
using lightClinic.Patient.Shared.Enums;
using Microsoft.AspNetCore.Identity;

namespace lightClinic.Patient.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, ISoftDelete, IAuditableEntity
    {
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public Language Language { get; set; }
        public bool NotificationEnabled { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Profile> Profiles { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Card> Cards { get; set; }
        public ICollection<NotificationToken> NotificationTokens { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        public ApplicationUser()
        {
            Id = Guid.CreateVersion7();
        }
    }
}