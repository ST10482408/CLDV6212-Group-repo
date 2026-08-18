using Azure.Storage.Files.Shares;
using CoffeeAndChill.Interface;
using CoffeeAndChill.Models;

namespace CoffeeAndChill.Services;

public class DocumentStorageService : IDocumentStorageService
{
    private readonly ShareClient _shareClient;
    private const string ShareName = "staff-docs";

    public DocumentStorageService(string connectionString)
    {
        _shareClient = new ShareClient(connectionString, ShareName);
        _shareClient.CreateIfNotExists();
    }

    public async Task UploadDocumentAsync(string fileName, Stream fileStream, CancellationToken ct = default)
    {
        var rootDir = _shareClient.GetRootDirectoryClient();
        var fileClient = rootDir.GetFileClient(fileName);

        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms, ct);
        ms.Position = 0;

        await fileClient.CreateAsync(ms.Length, cancellationToken: ct);
        await fileClient.UploadAsync(ms, cancellationToken: ct);
    }

    public async Task<List<StaffDocumentInfo>> ListDocumentsAsync(CancellationToken ct = default)
    {
        var rootDir = _shareClient.GetRootDirectoryClient();
        var results = new List<StaffDocumentInfo>();

        await foreach (var item in rootDir.GetFilesAndDirectoriesAsync(cancellationToken: ct))
        {
            if (!item.IsDirectory)
            {
                var fileClient = rootDir.GetFileClient(item.Name);
                var props = await fileClient.GetPropertiesAsync(ct);
                results.Add(new StaffDocumentInfo
                {
                    FileName = item.Name,
                    SizeInBytes = props.Value.ContentLength,
                    LastModified = props.Value.LastModified
                });
            }
        }
        return results;
    }

    public async Task<Stream?> DownloadDocumentAsync(string fileName, CancellationToken ct = default)
    {
        var rootDir = _shareClient.GetRootDirectoryClient();
        var fileClient = rootDir.GetFileClient(fileName);

        if (!await fileClient.ExistsAsync(ct))
            return null;

        var download = await fileClient.DownloadAsync(cancellationToken: ct);
        return download.Value.Content;
    }
}