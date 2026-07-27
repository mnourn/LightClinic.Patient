using Microsoft.AspNetCore.Identity;

namespace lightClinic.Patient.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid> 
    {
        public ApplicationRole()
        {
            Id = Guid.CreateVersion7();
        }
    }
}
