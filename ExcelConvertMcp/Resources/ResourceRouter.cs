using ModelContextProtocol.Protocol;

namespace ExcelConvertMcp.Resources;

public static class ResourceRouter
{
    
    private static Dictionary<string, Dictionary<string, IResourceHandler>> Handlers { get; } = new()
    {
        {
            "test_file://", new Dictionary<string, IResourceHandler>()
            { 
                { "/Users/fblood53/Test/Excel", new ExcelResourceHandler() }
            } 
        },
    };
    
    private static (string Protocol, string Address) ParseUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentException("URI는 null이거나 빈 문자열일 수 없습니다.", nameof(uri));
        }
        
        int separatorIndex = uri.IndexOf("://");
        
        if (separatorIndex == -1)
        {
            throw new FormatException($"URI '{uri}'는 유효한 프로토콜 형식이 아닙니다.");
        }
        
        string protocol = uri.Substring(0, separatorIndex + 3);
        string address = uri.Substring(separatorIndex + 3);

        return (protocol, address);
    }

    public static async Task<ReadResourceResult> GetHandlerResourceAsync(string uri, CancellationToken ct = default)
    {
        var (protocol, address) = ParseUri(uri);
        
        if (Handlers.TryGetValue(protocol, out var handlers))
        {
            if (handlers.TryGetValue(address, out var handler))
            {
                return await handler.GetResourceAsync(uri, address, ct);
            }
            
            throw new KeyNotFoundException($"URI '{uri}'에 대한 핸들러를 찾을 수 없습니다.");
            
        }
        
        throw new NotSupportedException($"지원되지 않는 프로토콜 '{protocol}'입니다.");
    }
}

public interface IResourceHandler
{
    Task<ReadResourceResult> GetResourceAsync(string fullUri, string realUri, CancellationToken ct = default);
}