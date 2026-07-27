namespace lightClinic.Patient.Contracts.DTOs.OTP
{
    public class OtpDto
    {
        public long OtpId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
