using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Persistence.Contexts;
using lightClinic.Patient.Persistence.Repositories;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Persistence.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddMemoryCache();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PatientAuthDbContext>(options =>
           options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(PatientAuthDbContext).Assembly.FullName)).LogTo(Console.WriteLine));

        // Identity configuration
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(ApplicationConstants.LockoutDurationMinutes);
            options.Lockout.MaxFailedAccessAttempts = ApplicationConstants.LockoutMaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<PatientAuthDbContext>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddDefaultTokenProviders();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IGovernmentRepository), typeof(GovernmentRepository))
            .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}