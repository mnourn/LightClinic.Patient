using FluentValidation;
using lightClinic.Patient.Application.Services.Notification;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace lightClinic.Patient.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddValidators();
        services.AddAutoMapper();
        services.AddNotificationServices();
    }

    private static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        // Register background service for medication reminders
        services.AddHostedService<MedicationReminderBackgroundService>();

        return services;
    }
}