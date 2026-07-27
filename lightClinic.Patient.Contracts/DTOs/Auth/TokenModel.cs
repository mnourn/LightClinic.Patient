namespace lightClinic.Patient.Contracts.DTOs.Auth;

public record TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
