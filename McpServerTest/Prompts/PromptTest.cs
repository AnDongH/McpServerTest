using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace McpServerTest.Prompts;

[McpServerPromptType]
public static class PromptTest
{
    
    [McpServerPrompt, Description("code 에 대해서 설명해주는 프롬프트")]
    public static ChatMessage TestPrompt([Description("코드 내용")]string code, [Description("언어")]string language)
    {
        return new (ChatRole.User, $"해당 코드에 대해서 자세히 설명해줘. {language} 언어로 작성된 코드: {code} 설명을 끝마친 후에는 'END'라고 말해줘.");
    }
    
    [McpServerPrompt, Description("내장 리소스 테스트 프롬프트")]
    public static ChatMessage TestResourcePrompt([Description("코드 내용")]string code, [Description("언어")]string language)
    {
        return new (ChatRole.User, $"해당 코드에 대해서 자세히 설명해줘. {language} 언어로 작성된 코드: {code} 설명을 끝마친 후에는 'END'라고 말해줘.");
    }
    
}
