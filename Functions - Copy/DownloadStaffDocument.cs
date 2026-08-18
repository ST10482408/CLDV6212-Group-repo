using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using CoffeeAndChill.Interface;

namespace CoffeeAndChill.Functions;

public class DownloadStaffDocument
{
    private readonly IDocumentStorageService _storage;

    public DownloadStaffDocument(IDocumentStorageService storage) => _storage = storage;

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents/download/{fileName}")] HttpRequest req,
        string fileName)
    {
        var stream = await _storage.DownloadDocumentAsync(fileName);
        if (stream == null)
            return new NotFoundObjectResult($"File '{fileName}' not found.");

        return new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }
}