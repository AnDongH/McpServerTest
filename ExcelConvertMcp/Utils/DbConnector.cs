using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using DbType = ExcelConvertMcp.Tools.DbType;

namespace ExcelConvertMcp;

public static class DbConnector
{
    public static IDbConnection GetDbConnection(DbType dbType, string connectionString)
    {
        return dbType switch
        {
            DbType.MySql => new MySqlConnection(connectionString),
            DbType.PgSql => new NpgsqlConnection(connectionString),
            DbType.SqlServer => new SqlConnection(connectionString),
            DbType.SqlLite => new SqliteConnection(connectionString),
            _ => throw new NotSupportedException($"Database type {dbType} is not supported.")
        };
    }

}