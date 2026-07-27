namespace lightClinic.Patient.Contracts.Interfaces.Auth;

public interface ICurrentUser
{
    Guid GetUserId();
}
