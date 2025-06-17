namespace ExcelConvertMcp.Utils;

public class JsonData
{
    public string TableName { get; set; }
    public List<JsonColumnData> Columns { get; set; }
    public List<string> InsertDatas { get; set; }
}

public class JsonColumnData
{
    public string ColumnName { get; set; }
    public string CSharpType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsValidForServer { get; set; }
    public bool IsValidForClient { get; set; }

    public JsonColumnData(ColumnData column)
    {
        ColumnName = CaseConverter.Converters.ToCamelCase(column.Name);
        CSharpType = column.CsType;
        IsNullable = column.IsNullable;
        IsValidForServer = column.IsValidForServer;
        IsValidForClient = column.IsValidForClient;
    }
    
}