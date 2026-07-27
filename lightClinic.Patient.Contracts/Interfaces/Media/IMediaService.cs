using lightClinic.Patient.Contracts.DTOs.Media;
using Microsoft.AspNetCore.Http;

namespace lightClinic.Patient.Contracts.Interfaces;

public interface IMediaService
{
    Task<(bool Succeed, string Message)> SaveAsync(IFormFile media);

    Task<ConfirmResultDto> ConfirmAsync(string filename);

    string GetBlobUrl(string blobName);
}