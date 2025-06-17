using System.ComponentModel;
using Dapper;
using ModelContextProtocol.Server;

namespace ExcelConvertMcp.Tools;

public enum DbType
{
    MySql,
    PgSql,
    SqlServer,
    SqlLite
}

[McpServerToolType]
public static class SqlTools
{

    [McpServerTool, Description("원시 SQL 쿼리 실행해주는 도구")]
    public static async Task ExecuteQuery([Description("문자열로 된 쿼리문")]string query, [Description("커넥션 스트링")]string connectionString, [Description("DB 타입")]DbType dbType)
    {
        using (var connection = DbConnector.GetDbConnection(dbType, connectionString))
        {
            connection.Open();
            await connection.ExecuteAsync(query);
        }
    }
    
}