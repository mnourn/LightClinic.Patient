using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities
{
    public class AuthOtp : BaseAuditableEntity
    {
        public int Otp { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}