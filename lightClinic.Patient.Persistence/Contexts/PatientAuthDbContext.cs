using lightClinic.Patient.Domain.Common;
using lightClinic.Patient.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;
using System.Security.Claims;

namespace lightClinic.Patient.Persistence.Contexts
{
    public class PatientAuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientAuthDbContext> _logger;

        public PatientAuthDbContext(
            DbContextOptions<PatientAuthDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientAuthDbContext> logger) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Government> Governments { get; set; }
        public DbSet<RelatedPatient> RelatedPatients { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationReminder> MedicationReminders { get; set; }
        public DbSet<MedicationIntakeTime> MedicationIntakeTimes { get; set; }
        public DbSet<ChronicDisease> ChronicDiseases { get; set; }
        public DbSet<DosageLookup> DosageLookups { get; set; }
        public DbSet<MedicineLookup> MedicineLookups { get; set; }
        public DbSet<UnitLookup> MedicineUnitLookups { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("dbo");
            builder.Ignore<IdentityRoleClaim<Guid>>();
            builder.Ignore<IdentityUserLogin<Guid>>();
            builder.Ignore<IdentityUserClaim<Guid>>();
            builder.Ignore<IdentityUserToken<Guid>>();
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //AuditHelper.BeforeSaveChanges(this, AuditLogs);
            var claim = _httpContextAccessor.HttpContext is null || _httpContextAccessor.HttpContext.User is null ?
                null : _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var entity in ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseAuditableEntity && x.State == EntityState.Added)
                .Select(x => x.Entity)
                .Cast<BaseAuditableEntity>())
            {
                entity.CreatedDate = DateTime.Now;
                if (claim is not null && entity.CreatedBy is null)
                {
                    entity.CreatedBy = Guid.Parse(claim.Value);
                }
            }
            foreach (var entity in ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseAuditableEntity && x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .Cast<BaseAuditableEntity>())
            {
                if (entity.IsDeleted)
                {
                    entity.DeletedAt = DateTime.Now;
                    if (claim is not null)
                    {
                        entity.DeletedBy = Guid.Parse(claim.Value);
                    }
                }
                else
                {
                    entity.LastModificationTime = DateTime.Now;
                    if (claim is not null)
                    {
                        entity.LastModifierId = Guid.Parse(claim.Value);
                    }
                }
            }

            try
            {
                int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database Save Changes Exception");

                throw new Exception("Connection timeout, please try again.");
            }
        }
    }
}