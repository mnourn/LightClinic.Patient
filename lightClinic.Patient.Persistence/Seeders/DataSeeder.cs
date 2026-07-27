using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Persistence.Seeders;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<PatientAuthDbContext>();

        if (!dbContext.Governments.Any())
        {
            dbContext.Governments.AddRange(
            [
                new Government() { Name = "Alexandria", Cities = new [] { new City() { Name = "Alexandria" } } },
                new Government() { Name = "Assiut", Cities = new [] { new City() { Name = "Assiut" } } },
                new Government() { Name = "Aswan", Cities = new [] { new City() { Name = "Aswan" } } },
                new Government() { Name = "Beheira", Cities = new [] { new City() { Name = "Beheira" } } },
                new Government() { Name = "Bani Suef", Cities = new [] { new City() { Name = "Bani Suef" } } },
                new Government() { Name = "Cairo", Cities = new [] { new City() { Name = "Cairo" } } },
                new Government() { Name = "Giza", Cities = new [] { new City() { Name = "Giza" } } },
            ]);
        }

        await dbContext.SaveChangesAsync();
    }
}

