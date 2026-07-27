using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public interface IMinimalEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}
