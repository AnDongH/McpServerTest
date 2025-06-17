using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace ExcelConvertMcp.Prompts;

[McpServerPromptType]
public static class ExcelConvertPrompt
{
    
    [McpServerPrompt, Description("내장 리소스를 활용해서 db 테이블을 생성하는 프롬프트")]
    public static IEnumerable<ChatMessage> CreateTablePrompt(
        [Description("데이터베이스 타입. ex - MySql, PgSql, SqLite..")] string dbType, 
        [Description("데이터베이스 host")]string ip, 
        [Description("데이터베이스 port")]string port, 
        [Description("데이터베이스 유저 이름")]string userName, 
        [Description("데이터베이스 접근 비밀 번호")]string password,
        [Description("접근할 데이타베이스 이름")]string database,
        [Description("데이터를 넣을 때 한번에 넣을 사이즈")]int batchSize)
    {
        List<ChatMessage> messages = new();

        messages.Add(new ChatMessage(ChatRole.User, "목표: 너는 지금부터 내가 첨부한 내장 리소스를 활용해서 db 테이블을 생성하는 역할을 맡게 될거야."));
        messages.Add(new ChatMessage(ChatRole.Assistant, "네 알겠습니다! 저는 첨부된 내장 리소스를 활용해서 db 테이블을 생성하는 에이전트입니다! 구체적인 정보를 주십시오"));
        
        messages.Add(new ChatMessage(ChatRole.User, $"사전 지식: " +
                                                           $"1. 먼저 {dbType} 에 따라서 테이블을 생성하기 위한 쿼리가 달라야해. " +
                                                           $"예를 들어서 mysql, pgsql, sqlite 등등의 경우 각각의 쿼리가 달라." +
                                                           $"2. 내장 리소스는 Json 형태로 테이블 이름과, Column 이름, C# 타입, Nullable 여부, 서버에서 사용 가능한지 여부, 클라이언트에서 사용 가능한지 여부를 포함하고 있어" +
                                                           $"그리고 넣어야 할 데이터들이 InsertDatas 라는 이름으로 들어가 있어."));
        messages.Add(new ChatMessage(ChatRole.Assistant, $"네 알겠습니다! {dbType}에 따라서 쿼리 문법을 적합하게 생성하겠습니다!, 그리고 내장 리소스의 Column 이름과 C# 타입, Nullable 여부, 서버에서 사용 가능한지 여부, 클라이언트에서 사용 가능한지 여부를 활용하겠습니다!" +
                                                                $"그리고 주어주신 데이터들을 테이블 안에 넣도록 하겠습니다!"));
        
        
        
        messages.Add(new ChatMessage(ChatRole.User, $"할 일: " +
                                                           $"1. {ip}, {port}, {userName}, {password}, {database} 를 이용해서 Connection String을 이용해서 Db에 접근해" +
                                                           $"2. 만약 {database}가 없다면, 생성해." +
                                                           $"3. C# 타입과 {dbType}을 확인해서 적당한 타입으로 변환해서 컬럼의 타입으로 넣어줘. " +
                                                           $"이때 문자열은 128자로 제한해줘." +
                                                           $"그리고 C#의 decimal, decimal?과 같은 경우 (20, 2)를 기준으로 해줘." +
                                                           $"예를 들어서 문자열은 mysql이나, pgsql의 경우 varchar(128)로" +
                                                           $"decimal, decimal?은 mysql의 경우 decimal(20, 2), pgsql의 경우 numeric(20, 2). 이 되는거야." +
                                                           $"4. 첫번째 Column을 Primary Key로 설정해줘." +
                                                           $"5. Nullable 여부를 확인하고, true이면 Nullable로 설정해줘. false이면 Not Null로 설정해줘." +
                                                           $"6. 이제 테이블을 생성하는 쿼리를 작성해서 Db에 넣어." +
                                                           $"7. 이때 테이블이 이미 존재하는 테이블이라면, 삭제하고 다시 생성해." +
                                                           $"테이블을 생성할 때는 utf8mb4 문자셋을 지원하도록 해" +
                                                           $"8. InsertDatas를 확인해서, 테이블에 넣어줘. 이때 데이터를 {batchSize}개 단위로 넣어" +
                                                           $"만약 데이터의 수가 {batchSize}개가 되지 않는다면 나머지를 전부 한번에 넣어." +
                                                           $"9. 작업을 하기 위한 mcp 도구로 excelTool의 ExecuteQuery 를 사용해." +
                                                           $"이때 dbType은 {dbType}을 보고 정해."));
        messages.Add(new ChatMessage(ChatRole.User, "단계별로 정확하게 진행해"));
        
        messages.Add(new ChatMessage(ChatRole.Assistant, "네 알겠습니다! 주어진 정보를 바탕으로 테이블을 생성하는 쿼리를 작성하고, 직접 Db에 넣겠습니다! 작업을 하기 위한 도구로 excelTool의 ExecuteQuery 를 사용하겠습니다!"));
        
        
        
        return messages;
    }
}