using System.ComponentModel;
using ExcelConvertMcp.Tools;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace ExcelConvertMcp.Prompts;

[McpServerPromptType]
public static class TestPrompt
{
    [McpServerPrompt, Description("테스트 Sql 프롬프트")]
    public static IEnumerable<ChatMessage> GetTestSqlPrompt(
        [Description("")]string dbType,
        [Description("데이터베이스 host")] string ip,
        [Description("데이터베이스 port")] string port,
        [Description("데이터베이스 유저 이름")] string userName,
        [Description("데이터베이스 접근 비밀 번호")] string password,
        [Description("접근할 데이타베이스 이름")] string database)
    {
        var message = new List<ChatMessage>();

        message.Add(new ChatMessage(ChatRole.User, "너는 내가 Sql을 테스트해주는 에이전트야."));
        message.Add(new ChatMessage(ChatRole.Assistant, "네! 알겠습니다. 저는 Sql을 테스트해주는 에이전트입니다! 구체적인 정보를 주십시오"));
        
        message.Add(new ChatMessage(ChatRole.User, "사전 지식: " +
                                                       $"1. 먼저 {dbType} 에 따라서 테이블을 생성하기 위한 쿼리가 달라야해. " +
                                                       $"예를 들어서 mysql, pgsql, sqlite 등등의 경우 각각의 쿼리가 달라." +
                                                       $"2. {ip}, {port}, {userName}, {password}, {database} 를 이용해서 Connection String을 이용해서 Db에 접근해" +
                                                       $"3. testPrompt라는 테이블이 있는지 확인해." +
                                                       $"4. 있으면 삭제하고 새로 만들어. 테이블 스키마는 알아서 정해" +
                                                       $"5. 없으면 새로 만들어. 테이블 스키마는 알아서 정해" +
                                                       $"6. 만들었다면 더미 데이터 10개 넣어."));
        
        message.Add(new ChatMessage(ChatRole.User, "단계별로 정확하게 진행해"));
        
        message.Add(new ChatMessage(ChatRole.Assistant, "네! 알겠습니다. 말씀해주신 내용을 수행하겠습니다!"));
        
        return message;
    }
}