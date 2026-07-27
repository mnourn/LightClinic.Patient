using Amazon.S3;
using Amazon.S3.Model;
using lightClinic.Patient.Contracts.DTOs.Media;
using lightClinic.Patient.Contracts.Interfaces;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace lightClinic.Patient.Infrastructure.Services.Media;

public class S3MediaService : IMediaService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _documentTempPath;
    private readonly string[] _allowedExtensions = ["gif", "jpg", "jpeg", "bmp", "png", "pdf", "doc", "docx"];

    public S3MediaService(IConfiguration configuration)
    {
        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];
        var region = configuration["AWS:Region"] ?? "us-east-1";
        _bucketName = configuration["AWS:BucketName"];
        _documentTempPath = configuration["AWS:DocumentTempPath"];

        if (string.IsNullOrWhiteSpace(_bucketName))
            throw new InvalidOperationException("AWS:BucketName configuration is missing or empty.");
        if (string.IsNullOrWhiteSpace(_documentTempPath))
            throw new InvalidOperationException("AWS:DocumentTempPath configuration is missing or empty.");

        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
        };

        _s3Client = !string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey) ?
            new AmazonS3Client(accessKey, secretKey, config) : new AmazonS3Client(config);
    }

    public async Task<(bool Succeed, string Message)> SaveAsync(IFormFile media)
    {
        if (media is null) return (false, "Empty File!");
        if (media.Length > ApplicationConstants.MaxUploadedFileSize)
        {
            double megabytes = ApplicationConstants.MaxUploadedFileSize / 1048576.0;
            return (false, $"File Size exceeds {megabytes.ToString("F2")} MB!");
        }

        try
        {
            string extension = Path.GetExtension(media.FileName).TrimStart('.');
            string error = ValidateExtension(extension);
            if (!string.IsNullOrWhiteSpace(error))
                return (false, error);
            string file = Guid.CreateVersion7().ToString();

            string folder = Path.Combine(Directory.GetCurrentDirectory(), _documentTempPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filename = $"{file}.{extension}";
            using (Stream fileStreams = new FileStream(Path.Combine(folder, filename), FileMode.Create))
            {
                await media.CopyToAsync(fileStreams);
                await fileStreams.FlushAsync();
            }

            return (true, filename);
        }
        catch (Exception ex)
        {
            return (false, $"Failed to save file: {ex.Message}");
        }
    }

    public async Task<ConfirmResultDto> ConfirmAsync(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return new ConfirmResultDto(true, null);
        if (!filename.Contains('.') || !Guid.TryParse(filename.Split('.')[0], out _))
            return new ConfirmResultDto(false, "Invalid uploaded file!");

        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), _documentTempPath, filename);
            if (!File.Exists(path))
                return new ConfirmResultDto(false, "File not found!");
            string res = string.Empty;
            using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                res = await UploadAsync(fsSource, filename);
            }
            File.Delete(path);
            DeleteExpiredFiles();

            return new ConfirmResultDto(true, res);
        }
        catch (Exception ex)
        {
            return new ConfirmResultDto(false, $"Failed to upload file: {ex.Message}");
        }
    }

    public string GetBlobUrl(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return null;

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = blobName,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(ApplicationConstants.DownloadFileExpiryHours)
            };

            return _s3Client.GetPreSignedURL(request);
        }
        catch
        {
            return null;
        }
    }

    private void DeleteExpiredFiles()
    {
        Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), _documentTempPath))
            .Select(f => new FileInfo(f))
            .Where(f => f.CreationTime < DateTime.Now.AddHours(ApplicationConstants.UploadedFileExpiryHours))
            .ToList()
            .ForEach(f => f.Delete());
    }

    private async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = GetContentType(fileName)
        };

        await _s3Client.PutObjectAsync(request);

        return fileName;
    }

    private string ValidateExtension(string extension)
    {
        if (extension.Contains(' ') || extension.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            return "Invalid file extension!";
        }

        return _allowedExtensions.Any(x => string.Equals(x, extension, StringComparison.OrdinalIgnoreCase))
            ? null
            : "Invalid file type!";
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}
