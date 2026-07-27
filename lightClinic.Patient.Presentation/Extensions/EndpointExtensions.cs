using lightClinic.Patient.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace lightClinic.Patient.Presentation.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection services)
    {
        var assembly = typeof(IMinimalEndpoint).Assembly;
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => !type.IsAbstract &&
                           !type.IsInterface &&
                           type.IsAssignableTo(typeof(IMinimalEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IMinimalEndpoint), type));
        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder RegisterMinimalEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IMinimalEndpoint>>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapRoutes(app);
        }

        return app;
    }
}
