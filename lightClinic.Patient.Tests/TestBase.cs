using lightClinic.Patient.Application.Extensions;
using lightClinic.Patient.Contracts.DTOs.Address;
using lightClinic.Patient.Contracts.DTOs.Card;
using lightClinic.Patient.Contracts.DTOs.Media;
using lightClinic.Patient.Contracts.DTOs.Notification;
using lightClinic.Patient.Contracts.Interfaces;
using lightClinic.Patient.Contracts.Interfaces.Address;
using lightClinic.Patient.Contracts.Interfaces.Card;
using lightClinic.Patient.Contracts.Interfaces.City;
using lightClinic.Patient.Contracts.Interfaces.Notification;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Infrastructure.Extensions;
using lightClinic.Patient.Persistence.Contexts;
using lightClinic.Patient.Persistence.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Enums;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace lightClinic.Patient.Tests;

public class TestBase
{
    protected WebApplicationBuilder _builder { get; set; }
    protected IServiceProvider _serviceProvider { get; set; }
    protected IConfiguration _configuration { get; set; }
    public Guid PatientId { get; set; }
    public Guid PatientProfileId { get; set; }

    [TestInitialize]
    public void Setup()
    {
        PatientId = Guid.NewGuid();
        PatientProfileId = Guid.NewGuid();
        _builder = WebApplication.CreateBuilder();
        _builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _builder.Services.AddMemoryCache();
        _builder.Services.AddHttpContextAccessor();
        _builder.Services.AddApplicationLayer();
        _builder.Services.AddInfrastructureLayer(_builder.Configuration);
        AddRepositories(_builder.Services);
        AddDbContext(_builder.Services, _builder.Configuration);
        _builder.Services.AddSingleton<INotificationApi>(MockINotificationApi());
        _builder.Services.AddSingleton<IMediaService>(MockMediaService());

        _serviceProvider = _builder.Services.BuildServiceProvider();
        _configuration = _builder.Configuration;

        SeedDatabaseAsync().ConfigureAwait(false);
    }

    [TestCleanup]
    public void Cleanup()
    {
        var dbContext = _serviceProvider.GetService<PatientAuthDbContext>();

        dbContext.Database.EnsureDeleted();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services
            .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }

    public void Login(Guid? userId = null)
    {
        var httpContext = new DefaultHttpContext();
        var identity = new GenericIdentity("user");
        identity.AddClaims(
        [
            new Claim(ClaimTypes.NameIdentifier, (userId ?? PatientId).ToString()),
            new Claim(ClaimTypes.Email, "admin@example.com"),
            new Claim(ClaimTypes.Name, "admin@example.com"),
            new Claim(ClaimTypes.Role, ApplicationConstants.PatientRoleName),
        ]);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor
            .SetupGet(accessor => accessor.HttpContext)
            .Returns(httpContext);
        httpContext.User.AddIdentity(identity);

        _builder.Services.AddSingleton<IHttpContextAccessor>(httpContextAccessor.Object);
        _serviceProvider = _builder.Services.BuildServiceProvider();
    }

    public IFormFile MockTextFile()
    {
        var fileName = "testFile.png";
        var fileContent = "This is a test file.";
        var contentBytes = Encoding.UTF8.GetBytes(fileContent);
        var stream = new MemoryStream(contentBytes);

        // Mock the IFormFile
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns(fileName);
        formFileMock.Setup(f => f.Length).Returns(stream.Length);
        formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        formFileMock.Setup(f => f.ContentType).Returns("text/plain");

        return formFileMock.Object;
    }

    public IFormFile MockImageFile()
    {
        // Mock the IFormFile
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("mockImage.png");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.ContentType).Returns("image/png");

        return mockFile.Object;
    }

    public INotificationApi MockINotificationApi()
    {
        var mockApi = new Mock<INotificationApi>();
        var resultCreateEmail = Result<bool>.Success(true);
        string responseCreateEmail = JsonConvert.SerializeObject(resultCreateEmail);
        mockApi.Setup(api => api.CreateEmailAsync(It.IsAny<EmailRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseCreateEmail);
        mockApi.Setup(api => api.CreateSmsAsync(It.IsAny<SmsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseCreateEmail);

        return mockApi.Object;
    }

    public IMediaService MockMediaService()
    {
        var mockMediaService = new Mock<IMediaService>();

        // Optionally, setup behavior
        mockMediaService
            .Setup(q => q.ConfirmAsync(It.IsAny<string>()))
            .ReturnsAsync(new ConfirmResultDto(true, Guid.NewGuid().ToString()));

        return mockMediaService.Object;
    }

    public async Task<IResult<AddressDto>> CreateAddressAsync(string phoneNumber = null, LocationType? locationType = null, string street = null, string buildingName = null, byte? floor = null, byte? aptNo = null, string additionalDirections = null, string location = null)
    {
        var cityService = _serviceProvider.GetRequiredService<ICityService>();
        var cities = await cityService.GetAllAsync(CancellationToken.None);
        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var dto = new CreateAddressDto()
        {
            PhoneNumber = phoneNumber ?? "01024765979",
            LocationType = locationType ?? LocationType.Home,
            Street = street ?? "Street",
            BuildingName = buildingName ?? "BuildingName",
            Floor = floor ?? 1,
            AptNo = aptNo ?? 2,
            AdditionalDirections = additionalDirections ?? "AdditionalDirections",
            Location = location ?? "Location",
            CityId = cities.Data.FirstOrDefault()?.Id,
            Longitude = 30.123456,
            Latitude = 31.123456
        };

        return await service.CreateAsync(dto, CancellationToken.None);
    }

    public async Task<IResult<AddressDto>> UpdateAddressAsync(long id, string phoneNumber = null, LocationType? locationType = null, string street = null, string buildingName = null, byte? floor = null, byte? aptNo = null, string additionalDirections = null, string location = null)
    {
        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var dto = new UpdateAddressDto()
        {
            Id = id,
            PhoneNumber = phoneNumber ?? "01024765979",
            LocationType = locationType ?? LocationType.Home,
            Street = street ?? "Street",
            BuildingName = buildingName ?? "BuildingName",
            Floor = floor ?? 1,
            AptNo = aptNo ?? 2,
            AdditionalDirections = additionalDirections ?? "AdditionalDirections",
            Location = location ?? "Location",
        };

        return await service.UpdateAsync(dto, CancellationToken.None);
    }

    public async Task<IResult<CardDto>> CreateCardAsync(long? cardNumber = null, string cardHolderName = null, int? cvvCode = null, DateOnly? expiryDate = null)
    {
        var service = _serviceProvider.GetRequiredService<ICardService>();
        var createCardDto = new CreateCardDto
        {
            CardNumber = cardNumber ?? 4111111111111111,
            CardHolderName = cardHolderName ?? "Visa",
            CVVCode = cvvCode ?? 1234,
            ExpiryDate = expiryDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        };
        return await service.CreateAsync(createCardDto, CancellationToken.None);
    }

    public async Task<IResult<CardDto>> UpdateCardAsync(long id, long? cardNumber = null, string cardHolderName = null, int? cvvCode = null, DateOnly? expiryDate = null)
    {
        var service = _serviceProvider.GetRequiredService<ICardService>();
        var updateCardDto = new UpdateCardDto
        {
            Id = id,
            CardNumber = cardNumber ?? 4111111111111111,
            CardHolderName = cardHolderName ?? "Visa",
            CVVCode = cvvCode ?? 1234,
            ExpiryDate = expiryDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        };
        return await service.UpdateAsync(updateCardDto, CancellationToken.None);
    }

    private void AddDbContext(IServiceCollection services, IConfiguration configuration, IHttpContextAccessor contextAccessor = null)
    {
        services.AddDbContext<PatientAuthDbContext>(options => options.UseInMemoryDatabase("TestDB"));

        // Identity configuration
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.SignIn.RequireConfirmedEmail = false;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<PatientAuthDbContext>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddDefaultTokenProviders();
    }

    private async Task<bool> SeedDatabaseAsync()
    {
        try
        {
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await userManager.Users.AnyAsync())
            {
                await userManager.CreateAsync(new ApplicationUser()
                {
                    Id = PatientId,
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    PhoneNumber = "01024765979",
                    EmailConfirmed = true,
                    Profiles = new List<Profile>() { new Profile() { PhoneNumber = "01024765979" } },
                    Addresses = new List<Domain.Entities.Address>()
                    {
                        new Domain.Entities.Address()
                        {
                            City = new City() { Name = "Cairo", Government = new Government() { Name = "Government" } },
                            BuildingName = "BuildingName",
                            Street = "Street",
                            Floor = 1,
                            AptNo = 2,
                            Location = "Location",
                            LocationType = LocationType.Home,
                            PhoneNumber = "01024765979",
                            AdditionalDirections = "AdditionalDirections",
                            Longitude = 30.123456,
                            Latitude = 31.123456
                        } 
                    }
                }, "123456a@A");
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
