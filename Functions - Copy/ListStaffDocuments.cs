using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using CoffeeAndChill.Interface;

namespace CoffeeAndChill.Functions;

public class ListStaffDocuments
{
    private readonly IDocumentStorageService _storage;

    public ListStaffDocuments(IDocumentStorageService storage) => _storage = storage;

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "documents")] HttpRequest req)
    {
        var files = await _storage.ListDocumentsAsync();
        return new OkObjectResult(files);
    }
}