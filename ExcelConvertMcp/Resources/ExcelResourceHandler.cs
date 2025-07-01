using ModelContextProtocol.Protocol;

namespace ExcelConvertMcp.Resources;

public class ExcelResourceHandler : IResourceHandler
{
    public async Task<ReadResourceResult> GetResourceAsync(string fullUri, string realUri, CancellationToken ct = default)
    {
        var files = await Task.Run(() => Directory.GetFiles(realUri, "*.xlsx", SearchOption.AllDirectories), ct);

        ReadResourceResult result = new ReadResourceResult();
        foreach (var file in files)
        {

            using var ecxelParser = new ExcelParser();
            ecxelParser.Load(file, null, 0, 3, 0);
            
            result.Contents.Add(new TextResourceContents()
            {
                Uri = fullUri,
                Text = ecxelParser.GetTableInfo(),
                MimeType = "text/plain",
            });
        }

        return result;
    }
}