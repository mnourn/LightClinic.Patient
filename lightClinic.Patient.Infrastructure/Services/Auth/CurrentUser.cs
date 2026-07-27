using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace lightClinic.Patient.Infrastructure.Services.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string InvalidUserSession = "Invalid Current User Session!";
    private const string InvalidClaim = "Invalid Current User Claim!";
    private const string InvalidUserIdClaim = "Invalid Current User Id Claim!";

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager) =>
        (_httpContextAccessor, _userManager) = (httpContextAccessor, userManager);

    public Guid GetUserId()
    {
        if (_httpContextAccessor is null ||
            _httpContextAccessor.HttpContext is null ||
            _httpContextAccessor.HttpContext.User is null)
        {
            throw new Exception(InvalidUserSession);
        }

        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null)
        {
            throw new Exception(InvalidClaim);
        }

        if (!Guid.TryParse(claim.Value, out Guid currentUserId))
        {
            throw new Exception(InvalidUserIdClaim);
        }

        return currentUserId;
    }
}