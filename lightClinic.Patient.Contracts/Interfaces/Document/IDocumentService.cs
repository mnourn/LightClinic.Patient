using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Http;

namespace lightClinic.Patient.Contracts.Interfaces.Document;

public interface IDocumentService
{
    Task<IResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken);
}
