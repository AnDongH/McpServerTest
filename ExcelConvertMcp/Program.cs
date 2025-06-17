using System.Text;
using ExcelConvertMcp.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Protocol.Types;

namespace ExcelConvertMcp;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: null);
        HashSet<string> subscriptions = [];
        
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithPromptsFromAssembly()
            .WithListResourcesHandler((ctx, ct) =>
            {
                return Task.FromResult(new ListResourcesResult
                {
                    Resources =
                    [
                        new Resource { Name = "ExcelResources", Description = "엑셀 -> DB 변환을 위한 엑셀 데이터들", Uri = "test_file:///Users/fblood53/Test/Excel" }
                    ]
                });
            })
            .WithReadResourceHandler(async (ctx, ct) =>
            {
                
                var uri = ctx.Params?.Uri;
                return await ResourceRouter.GetHandlerResourceAsync(uri, ct);
                
            }).WithSubscribeToResourcesHandler((ctx, ct) =>
            {
                var uri = ctx.Params?.Uri;

                if (uri is not null)
                {
                    subscriptions.Add(uri);
                }

                return Task.FromResult(new EmptyResult());
            })
            .WithUnsubscribeFromResourcesHandler((ctx, ct) =>
            {
                var uri = ctx.Params?.Uri;
                if (uri is not null) subscriptions.Remove(uri);
                return Task.FromResult(new EmptyResult());
            });

        builder.Services.AddSingleton(_ =>
        {
            var client = new HttpClient();
            return client;
        });
        
        builder.Services.AddSingleton(subscriptions);
        builder.Services.AddHostedService<SubscriptionMessageSender>();

        var app = builder.Build();
        
        await app.RunAsync();
    }
}