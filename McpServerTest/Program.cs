using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Protocol;

namespace McpServerTest;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: null);

        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithPromptsFromAssembly()
            .WithListResourcesHandler((ctx, ct) =>
            {
                return ValueTask.FromResult(new ListResourcesResult
                {
                    Resources =
                    [
                        new Resource { Name = "Resource1", Description = "A static resource with a numeric ID", Uri = "test_file:///Users/fblood53/Test/test.txt" },
                        new Resource { Name = "Resource2", Description = "A static resource with a numeric ID", Uri = "test_file2:///Users/fblood53/Test/test.json" }
                    ]
                });
            })
            .WithReadResourceHandler(async (ctx, ct) =>
            {
                var uri = ctx.Params?.Uri;
             
                if (uri.StartsWith("test_file://")) uri = uri.Replace("test_file://", "");
                if (uri.StartsWith("test_file2://")) uri = uri.Replace("test_file2://", "");
                
                var content = await File.ReadAllTextAsync(uri);

                return new ReadResourceResult
                {
                    Contents = 
                    [
                        new TextResourceContents
                        {
                            Uri = ctx.Params!.Uri!,
                            Text = content,
                            MimeType = "text/plain",
                        }
                    ]
                };
            });

        builder.Services.AddSingleton(_ =>
        {
            var client = new HttpClient();
            return client;
        });

        var app = builder.Build();
        
        await app.RunAsync();
    }
}