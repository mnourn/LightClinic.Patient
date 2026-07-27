using lightClinic.Patient.Contracts.Interfaces;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Document;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Http;

namespace lightClinic.Patient.Application.Services.Document;

public class DocumentService : IDocumentService, IApplicationService
{
    private readonly IMediaService _mediaService;

    public DocumentService(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<IResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var filename = await _mediaService.SaveAsync(file);

        return filename.Succeed ? Result<string>.Success(filename.Message, "File uploaded successfully") : Result<string>.Failure(filename.Message, System.Net.HttpStatusCode.BadRequest);
    }
}
