using System.Data;
using System.Text;
using ExcelDataReader;

namespace ExcelConvertMcp;

public class ExcelEx
{
	public static readonly char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

	static ExcelEx()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
	}

	public static DataSet Open(string path)
	{
		using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
		using (var reader = ExcelReaderFactory.CreateReader(stream))
			return reader.AsDataSet();
	}

	public static DataTable Open(string path, int sheetIndex)
	{
		using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
		using (var reader = ExcelReaderFactory.CreateReader(stream))
		using (var dataSet = reader.AsDataSet())
		{
			var table = dataSet.Tables[sheetIndex];
			return table;
		}
	}

	public static DataTable Open(string path, string sheetName)
	{
		using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
		using (var reader = ExcelReaderFactory.CreateReader(stream))
		using (var dataSet = reader.AsDataSet())
		{
			var table = dataSet.Tables[sheetName];
			return table;
		}
	}

	public static string ToPosition(int row, int column)
	{
		if (row++ < 0) return null;
		if (column++ < 0) return null;

		var list = new List<char>();

		do
		{
			int c = --column % 26;
			column = column / 26;
			list.Add((char)('A' + c));
		} while (column > 0);

		if (list.Count > 1) list.Reverse();

		return new String(list.ToArray()) + row.ToString();
	}

	public static void FromPosition(string pos, out int row, out int column)
	{
		int index = pos.IndexOfAny(numbers);
		if (index < 0) throw new ArgumentException();

		var c = pos.Substring(0, index);
		var r = pos.Substring(index, pos.Length - index);

		row = int.Parse(r) - 1;

		column = 0;
		for (int i = 0; i < index; i++)
		{
			column *= 26;
			column += (c[i] - 'A' + 1);
		}
		column--;
	}
}
