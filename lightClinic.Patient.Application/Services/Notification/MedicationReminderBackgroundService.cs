using lightClinic.Patient.Application.Interfaces.Notification;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace lightClinic.Patient.Application.Services.Notification;

public class MedicationReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MedicationReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

    public MedicationReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MedicationReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Medication Reminder Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing medication reminders");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Medication Reminder Background Service stopped");
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var firebaseService = scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();

        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Egypt Standard Time");
        var reminderWindowStart = now.AddMinutes(-1); // Check reminders from 1 minute ago
        var reminderWindowEnd = now.AddMinutes(1); // To 1 minute ahead

        // Get reminders that need to be sent
        var reminders = await unitOfWork.Repository<Domain.Entities.MedicationReminder>().Entities
            .Include(x => x.Medication)
            .ThenInclude(x => x.Profile)
            .Include(x => x.Medication)
            .ThenInclude(x => x.MedicineLookup)
            .Include(x => x.Medication)
            .ThenInclude(x => x.DosageLookup)
            .Include(x => x.Medication)
            .ThenInclude(x => x.UnitLookup)
            .Where(x
            => !x.IsDeleted
            && !x.IsTaken
            && !x.IsMissed
            && x.Medication.Status == Shared.Enums.MedicationStatus.Active
            && (x.SnoozedUntil == null || x.SnoozedUntil <= now)
            && x.ReminderSentAt == null
            && x.ScheduledDate == DateOnly.FromDateTime(now)
            && x.ScheduledTime >= TimeOnly.FromTimeSpan(reminderWindowStart.TimeOfDay)
            && x.ScheduledTime <= TimeOnly.FromTimeSpan(reminderWindowEnd.TimeOfDay))
            .ToListAsync(cancellationToken);

        foreach (var reminder in reminders)
        {
            try
            {
                var scheduledDateTime = reminder.ScheduledDate.ToDateTime(reminder.ScheduledTime);
                var medicationName = reminder.Medication.MedicineLookup?.Name ?? "Medication";
                var dosage = $"{reminder.Medication.DosageLookup?.Name ?? ""} {reminder.Medication.UnitLookup?.Name ?? ""}".Trim();

                var success = await firebaseService.SendMedicationReminderAsync(
                    reminder.Medication.ProfileId,
                    medicationName,
                    dosage,
                    scheduledDateTime,
                    cancellationToken);

                if (success)
                {
                    reminder.ReminderSentAt = DateTime.UtcNow;
                    await unitOfWork.SaveAsync(cancellationToken);
                    _logger.LogInformation(
                        "Sent medication reminder for Profile {ProfileId}, Medication {MedicationId}, Scheduled: {ScheduledDateTime}",
                        reminder.Medication.ProfileId, reminder.MedicationId, scheduledDateTime);
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to send medication reminder for Profile {ProfileId}, Medication {MedicationId}",
                        reminder.Medication.ProfileId, reminder.MedicationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending reminder for Medication {MedicationId}, Reminder {ReminderId}",
                    reminder.MedicationId, reminder.Id);
            }
        }

        // Mark missed reminders (past reminders that haven't been taken and weren't sent)
        var missedReminders = await unitOfWork.Repository<Domain.Entities.MedicationReminder>().Entities
            .Where(x
            => !x.IsDeleted
            && !x.IsTaken
            && !x.IsMissed
            && x.Medication.Status == Shared.Enums.MedicationStatus.Active
            && (x.ScheduledDate < DateOnly.FromDateTime(now)
            || (x.ScheduledDate == DateOnly.FromDateTime(now)
            && x.ScheduledTime < TimeOnly.FromTimeSpan(now.TimeOfDay.Add(TimeSpan.FromMinutes(-5)))))
            && (x.SnoozedUntil == null
            || x.SnoozedUntil < now))
            .ToListAsync(cancellationToken);

        if (missedReminders.Count > 0)
        {
            foreach (var reminder in missedReminders)
            {
                reminder.IsMissed = true;
            }
            await unitOfWork.SaveAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} reminders as missed", missedReminders.Count);
        }
    }
}
