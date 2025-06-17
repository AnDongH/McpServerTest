using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using ExcelConvertMcp.Utils;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace ExcelConvertMcp;

public class HeaderData
{
	public string Name { get; set; }
	public string Type { get; set; }
	public string Valid { get; set; }
}

public class ColumnData
{
	public string Name { get; private set; }
	public string CsType { get; private set; }
	public bool IsNullable { get; private set; }
	public bool IsValidForServer { get; private set; }
	public bool IsValidForClient { get; private set; }

	public ColumnData(string name, string csType, bool isNullable, bool isValidForServer, bool isValidForClient)
	{
		Name = name;
		CsType = csType;
		IsNullable = isNullable;
		IsValidForServer = isValidForServer;
		IsValidForClient = isValidForClient;
	}

	public override string ToString()
	{
		return $"Column Name : {Name}, C#Type : {CsType}, IsNullable : {IsNullable}, IsValidForServer : {IsValidForServer}, IsValidForClient : {IsValidForClient}";
	}
}

public class ExcelParser : IDisposable
{
	public string SheetName { get; private set; }
	public int SheetIndex { get; private set; }
	public int StartRow { get; private set; }
	public int StartColumn { get; private set; }
	public DataTable Table { get; private set; }
	public List<ColumnData> Columns { get; private set; }

	private bool isDisposed = false;
	
	public string GetTableInfo()
	{
		
		List<string> insertDatas = new List<string>();
		StringBuilder sb = new StringBuilder();
		for (int i = StartRow; i < Table.Rows.Count; i++)
		{
			var row = Table.Rows[i];
			var excelColumn = StartColumn;
			
			sb.Append("(");
			for (int c = 0; c < Columns.Count; c++, excelColumn++)
			{
				var column = Columns[c];
				if (!column.IsValidForServer) continue;
				
				string value = row[excelColumn].ToString();
				if (string.IsNullOrEmpty(value)) continue;

				sb.Append(value);
				if (c < Columns.Count - 1) sb.Append(", ");

			}
			sb.Append(")");
			insertDatas.Add(sb.ToString());
			sb.Clear();
		}
		
		JsonData jsonData = new JsonData
		{
			TableName = CaseConverter.Converters.ToCamelCase("tbl" + RemoveParenthesesAndContent(Table.TableName)),
			Columns = Columns.Where(x => x.IsValidForServer).Select(y => new JsonColumnData(y)).ToList(),
			InsertDatas = insertDatas
		};
		
		return JsonConvert.SerializeObject(jsonData);
	}

	private static Dictionary<string, string> dicExcelType2CsType = new()
	{
		{ "bool", "bool" },
		{ "sbyte", "sbyte" },
		{ "int8", "sbyte" },
		{ "byte", "byte" },
		{ "uint8", "byte" },
		{ "short", "short"},
		{ "int16", "short"},
		{ "ushort", "ushort" },
		{ "uint16", "ushort" },
		{ "int", "int" },
		{ "int32", "int" },
		{ "uint", "uint" },
		{ "uint32", "uint" },
		{ "long", "long" },
		{ "int64", "long" },
		{ "ulong", "ulong" },
		{ "uint64", "ulong" },
		{ "float", "float" },
		{ "double", "double" },
		{ "char", "char" },
		{ "DateTime", "DateTime" },
		{ "TimeSpan", "TimeSpan"},
		{ "time", "TimeSpan" },
		{ "decimal", "decimal" },
		{ "bool?", "bool?" },
		{ "sbyte?", "sbyte?" },
		{ "int8?", "sbyte?" },
		{ "byte?", "byte?" },
		{ "uint8?", "byte?" },
		{ "short?", "short?" },
		{ "int16?", "short?" },
		{ "ushort?", "ushort?" },
		{ "uint16?", "ushort?" },
		{ "int?", "int?" },
		{ "int32?", "int?" },
		{ "uint?", "uint?" },
		{ "uint32?", "uint?" },
		{ "long?", "long?" },
		{ "int64?", "long?" },
		{ "ulong?", "ulong?" },
		{ "uint64?", "ulong?" },
		{ "float?","float?" },
		{ "double?", "double?" },
		{ "char?", "char?" },
		{ "DateTime?", "DateTime?" },
		{ "TimeSpan?", "TimeSpan?" },
		{ "time?", "TimeSpan?" },
		{ "string", "string" },
		{ "decimal?", "decimal?" }
	};
	
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~ExcelParser()
	{
		Dispose(false);
	}

	protected virtual void Dispose(bool isDisposing)
	{
		if (isDisposed) return;

		if(isDisposing)
		{
			Columns = null;
			Table?.Dispose();
		}

		isDisposed = true;
	}

	// sheetName 이 null 이면 sheetIndex 를 사용
	public void Load(string path, string sheetName, int sheetIndex, int startRow, int startColumn)
	{
		if (!File.Exists(path))
			throw new Exception($"File not exists : {path}");
		var dataSet = ExcelEx.Open(path);
		Load(dataSet, sheetName, sheetIndex, startRow, startColumn);
	}

	public void Load(DataSet dataSet, string sheetName, int sheetIndex, int startRow, int startColumn)
	{
		if (dataSet == null)
			throw new Exception("DataSet is null");
		
		if (string.IsNullOrEmpty(sheetName))
			Table = dataSet.Tables[sheetIndex];
		else
			Table = dataSet.Tables[sheetName];

		SheetName = Table.TableName;
		SheetIndex = sheetIndex;
		StartRow = startRow;
		StartColumn = startColumn;

		var headers = ReadHeaders();
		Columns = ReadColumns(headers);
		CheckColumns();
	}
	
	private List<HeaderData> ReadHeaders()
	{
		if (Table.Rows.Count < 3) return null;

		var columnCount = Table.Columns.Count;
		var headers = new List<HeaderData>(columnCount);
		
		for (int c = 0; c < columnCount; c++)
		{
			if (c < StartColumn)
			{
				headers.Add(null);
			}
			else
			{
				var name = Table.Rows[0][c]?.ToString().Trim() ?? "";
				var type = Table.Rows[1][c]?.ToString().Trim().ToLower() ?? "";
				var valid = Table.Rows[2][c]?.ToString().Trim().ToLower() ?? "";
				
				headers.Add(new HeaderData { Name = name, Type = type, Valid = valid });
			}
		}

		return headers;
	}
	
	private List<ColumnData> ReadColumns(List<HeaderData> headers)
	{
		if (Table.Rows.Count < 3) return null;

		var columns = new List<ColumnData>();

		for (int c = StartColumn; c < Table.Columns.Count; c++)
		{
			var header = headers[c];

			bool isServerValid = true;
			bool isClientValid = true;

			if (string.IsNullOrEmpty(header.Name) || header.Name.StartsWith("#") || string.IsNullOrEmpty(header.Type) || !header.Valid.Contains("s"))
			{
				isServerValid = false;
				isClientValid = false;
			}
			
			if (!header.Valid.Contains("c")) isClientValid = false;
			
			bool isNullable = false;
			string csType = string.Empty;
			
			bool isConvertable = ConvertType(header.Type, out csType);
			
			if (!isConvertable)
			{
				if (isServerValid || isClientValid)
					throw new Exception(string.Format("Wrong Column Type : [{0}] {1}", c, header.Type));
			}
			
			columns.Add(new ColumnData(header.Name, csType, isNullable, isServerValid, isClientValid));
		}

		RemoveEmptyColumns(columns);

		return columns;
	}

	private void CheckColumns()
	{
		if (Columns == null || Columns.Count < 1)
			throw new Exception("There is NO column");
	}

	private void RemoveEmptyColumns(List<ColumnData> columns)
	{
		int emptyColumn = 0;

		foreach (var c in columns)
		{
			if (string.IsNullOrEmpty(c.Name))
				emptyColumn++;
			else
				emptyColumn = 0;
		}

		if (emptyColumn > 0)
		{
			columns.RemoveRange(columns.Count - emptyColumn, emptyColumn);
		}
	}

	static string RemoveParenthesesAndContent(string input)
	{
		return Regex.Replace(input, @"\([^()]*\)", "");
	}

	private bool ConvertType(string excelType, out string csType)
	{
		
		excelType = excelType.ToLower();
		csType = "?";

		if (excelType.Contains("datetime"))
		{
			excelType = excelType.Replace("datetime", "DateTime");
		}
		else if(excelType.Contains("timespan"))
		{
			excelType = excelType.Replace("timespan", "TimeSpan");
		}
		else if (excelType.Contains("time"))
		{
			excelType = excelType.Replace("time", "TimeSpan");
		}

		if (dicExcelType2CsType.ContainsKey(excelType))
		{
			csType = dicExcelType2CsType[excelType];
			return true;
		}

		if (excelType.StartsWith("string(") && excelType.EndsWith(")"))
		{
			int length = 0;

			if (int.TryParse(excelType.Substring(7, excelType.Length - 8), out length) == false) return false;
			if (length == 0) return false;
			
			csType = "string";

			return true;
		}

		if(excelType.StartsWith("varchar(") && excelType.EndsWith(")"))
		{
			int length = 0;

			if (int.TryParse(excelType.Substring(8, excelType.Length - 9), out length) == false) return false;
			if (length == 0) return false;
			
			csType = "string";

			return true;
		}

		if (excelType.StartsWith("decimal"))
		{
			csType = "decimal";
			return true;
		}

		return false;
	}
}
