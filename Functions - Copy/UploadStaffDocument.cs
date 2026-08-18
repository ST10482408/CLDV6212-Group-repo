using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CoffeeAndChill.Interface;

namespace CoffeeAndChill.Functions;

public class UploadStaffDocument
{
    private readonly IDocumentStorageService _storage;
    private readonly ILogger<UploadStaffDocument> _logger;

    public UploadStaffDocument(IDocumentStorageService storage, ILogger<UploadStaffDocument> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "documents/upload")] HttpRequest req)
    {
        if (!req.HasFormContentType || req.Form.Files.Count == 0)
            return new BadRequestObjectResult("No file uploaded.");

        var file = req.Form.Files[0];
        if (file.Length == 0)
            return new BadRequestObjectResult("Uploaded file is empty.");

        using var stream = file.OpenReadStream();
        await _storage.UploadDocumentAsync(file.FileName, stream);

        _logger.LogInformation("Uploaded {FileName}", file.FileName);
        return new OkObjectResult(new { message = "Uploaded", fileName = file.FileName });
    }
}