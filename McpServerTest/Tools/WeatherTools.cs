using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.AI;

namespace McpServerTest;

[McpServerToolType]
public static class WeatherTools
{
    [McpServerTool, Description("Get Naver Form in HTML format.")]
    public static async Task<string> GetNaverFormTest(HttpClient client)
    {
        using var m = await client.GetAsync("https://www.naver.com");

        string s = await m.Content.ReadAsStringAsync();
        
        return s;
    }

    [McpServerTool, Description("Get test mapping type")]
    public static string GetMappingTypeTest([Description("test mapping type")]TestMappingType testMappingType)
    {
        return testMappingType.Age + "," + testMappingType.Name + "OK";
    }
    
}

public class TestMappingType
{
    public string Name { get; set; }
    public int Age { get; set; }
}