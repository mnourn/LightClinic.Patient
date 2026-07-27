namespace lightClinic.Patient.Application.Interfaces.Notification;

public interface IFirebaseNotificationService
{
    Task<bool> SendPushNotificationAsync(string[] tokens, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
    Task<bool> SendMedicationReminderAsync(Guid profileId, string medicationName, string dosage, DateTime scheduledTime, CancellationToken cancellationToken = default);
}
