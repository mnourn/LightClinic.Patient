using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using lightClinic.Patient.Application.Interfaces.Notification;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace lightClinic.Patient.Application.Services.Notification;

public class FirebaseNotificationService : IFirebaseNotificationService, IApplicationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseNotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private bool _isInitialized = false;
    private readonly object _lockObject = new object();

    public FirebaseNotificationService(
        IConfiguration configuration,
        ILogger<FirebaseNotificationService> logger,
        IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _logger = logger;
        _unitOfWork = unitOfWork;
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        if (_isInitialized)
            return;

        lock (_lockObject)
        {
            if (_isInitialized)
                return;

            try
            {
                var firebaseConfigPath = _configuration.GetValue<string>("Firebase:ConfigPath");
                var firebaseProjectId = _configuration.GetValue<string>("Firebase:ProjectId");

                if (string.IsNullOrWhiteSpace(firebaseConfigPath) && string.IsNullOrWhiteSpace(firebaseProjectId))
                {
                    _logger.LogWarning("Firebase configuration not found. Push notifications will be disabled.");
                    return;
                }

                if (FirebaseApp.DefaultInstance == null)
                {
                    if (!string.IsNullOrWhiteSpace(firebaseConfigPath) && File.Exists(firebaseConfigPath))
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(firebaseConfigPath)
                        });
                    }
                    else if (!string.IsNullOrWhiteSpace(firebaseProjectId))
                    {
                        // Use default credentials (for cloud environments)
                        FirebaseApp.Create(new AppOptions()
                        {
                            ProjectId = firebaseProjectId
                        });
                    }
                }

                _isInitialized = true;
                _logger.LogInformation("Firebase initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase");
            }
        }
    }

    public async Task<bool> SendPushNotificationAsync(string[] tokens, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized || FirebaseApp.DefaultInstance == null)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send push notification.");
            return false;
        }
        var messages = GetMessages(tokens, title, body, data);
        if (messages != null)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages, cancellationToken);
                _logger.LogInformation("Sent {SuccessCount}/{TotalCount} push notifications for title '{Title}'", response.SuccessCount, messages.Count, title);
                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notifications for title '{Title}'", title);
            }
        }
        return false;
    }

    private List<Message> GetMessages(string[] tokens, string title, string body, Dictionary<string, string> data)
    {
        if (tokens is null || tokens.Length == 0)
            return new List<Message>();

        var messages = new List<Message>();
        var dataDic = data?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>();
        foreach (var token in tokens)
        {
            var message = new Message
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = dataDic,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        ChannelId = "medication_reminders"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = 1
                    }
                }
            };

            messages.Add(message);
        }

        return messages;
    }

    public async Task<bool> SendMedicationReminderAsync(Guid profileId, string medicationName, string dosage, DateTime scheduledTime, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokens = await _unitOfWork.Repository<NotificationToken>().Entities
                .Where(x => x.User.Profiles.Any(y => y.Id == profileId))
                .ToListAsync(cancellationToken);

            if (tokens.Count == 0)
            {
                _logger.LogWarning("No tokens found for profile {ProfileId}.", profileId);
                return false;
            }

            var title = "Medication Reminder";
            var body = $"Time to take {medicationName} - {dosage}";
            var data = new Dictionary<string, string>
            {
                { "type", "medication_reminder" },
                { "medicationName", medicationName },
                { "dosage", dosage },
                { "scheduledTime", scheduledTime.ToString("O") }
            };

            var sent = await SendPushNotificationAsync(tokens.Select(x => x.FCMToken).ToArray(), title, body, data, cancellationToken);
            if (sent)
            {
                var userId = await _unitOfWork.Repository<Domain.Entities.Profile>().Entities
                    .Where(x => x.Id == profileId)
                    .Select(x => x.UserId)
                    .FirstOrDefaultAsync(cancellationToken);

                await SaveNotificationAsync(userId, title, body, data, cancellationToken);
            }

            return sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name);
            return false;
        }
    }

    private async Task SaveNotificationAsync(Guid userId, string title, string body, Dictionary<string, string>? data, CancellationToken cancellationToken)
    {
        try
        {
            var notification = new Domain.Entities.Notification
            {
                UserId = userId,
                Title = title,
                Body = body,
                Data = data != null && data.Count > 0 ? JsonSerializer.Serialize(data) : null
            };
            await _unitOfWork.Repository<Domain.Entities.Notification>().AddAsync(notification);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save notification for user {UserId}", userId);
        }
    }
}
