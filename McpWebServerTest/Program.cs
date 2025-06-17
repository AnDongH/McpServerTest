
namespace McpWebServerTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMcpServer()
            .WithToolsFromAssembly();
        
        var app = builder.Build();
        
        app.Run();
    }
}