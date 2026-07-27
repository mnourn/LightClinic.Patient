using lightClinic.Patient.Contracts.Interfaces;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Notification;
using lightClinic.Patient.Infrastructure.Services.Identity;
using lightClinic.Patient.Infrastructure.Services.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Reflection;

namespace lightClinic.Patient.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        AddServices(services);
        AddRefit(services, configuration);
    }

    private static void AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IMediaService, S3MediaService>()
            .AddScoped<ICurrentUser, CurrentUser>();

        Type serviceType = typeof(IApplicationService);

        var types = Assembly.GetAssembly(serviceType).GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(serviceType))
            .ToList();

        foreach (var implementationType in types)
        {
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i != serviceType);

            if (interfaceType is not null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
    }

    private static void AddRefit(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Apis");
        services.AddRefitClient<INotificationApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(section["Notification"]));
    }
}