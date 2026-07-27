namespace lightClinic.Patient.Shared
{
    public static class ApplicationConstants
    {
        public const int OTPLength = 6;
        public const int TokenExpirationHours = 24;
        public const int RefreshTokenExpirationDays = 7;
        public const int RefreshTokenLength = 64;
        public const int MaxNameLength = 255;
        public const int MaxSubjectLength = 255;
        public const int MaxSenderLength = 15;
        public const int MaxUrlLength = 255;
        public const int MaxPhoneLength = 15;
        public const int MinPasswordLength = 9;
        public const int MaxPasswordLength = 50;
        public const int MaxAddressLength = 1000;
        public const int MaxFCMTokenLength = 500;
        public const int OTPExpirationSeconds = 300;
        public const int EmailMaxTrialCount = 2;
        public const int DefaultEmailCountToSend = 100;
        public const int MaxUploadedFileSize = 5242880; // 1024 * 1024 * 5 = 5 MB
        public const int UploadedFileExpiryHours = -24;
        public const int DownloadFileExpiryHours = 1;
        public const int AbsoluteExpirationHours = 1;
        public const int SlidingExpirationMinutes = 30;
        public const int MinSearchTextLength = 3;
        public const int MinWeight = 20;
        public const int MaxWeight = 635;
        public const int MinHeight = 60;
        public const int MaxHeight = 275;
        public const int LockoutMaxFailedAccessAttempts = 5;
        public const int LockoutDurationMinutes = 30;
        public const string PhoneNumberRegex = "^01[0125][0-9]{8}$";
        public const string DoctorRoleName = "Doctor";
        public const string PatientRoleName = "Patient";
        public const string NotificationApiRequestHeader = "X-Api-Key: J(DjKYPy%@8F2PXs";
        public const string AnonymousPolicyName = "Anonymous";
        public const string AuthenticatedPolicyName = "Authenticated";
    }
}