namespace G.Util;

public class FileEx
{
	public static string SearchFromApplicationDirectory(string fileName, int retryParentDirectory = 5)
	{
		var startDir = Path.GetDirectoryName(AppContext.BaseDirectory);
		return SearchParentDirectory(fileName, startDir, retryParentDirectory);
	}
	
	public static string SearchFromCurrentDirectory(string fileName, int retryParentDirectory = 5)
	{
		var startDir = Directory.GetCurrentDirectory();
		return SearchParentDirectory(fileName, startDir, retryParentDirectory);
	}

	public static string SearchFromFileDirectory(string filePath, int retryParentDirectory = 5)
	{
		string fileName = Path.GetFileName(filePath);
		string startDir = Path.GetDirectoryName(filePath);
		return SearchParentDirectory(fileName, startDir, retryParentDirectory);
	}
	
	public static string SearchParentDirectory(string fileName, string startDir, int retryParentDirectory = 5)
	{
		var path = Path.Combine(startDir, fileName);
		if (File.Exists(path))
			return path;

		string dir = startDir;

		for (int i = 1; i <= retryParentDirectory; i++)
		{
			dir = Path.Combine(dir, "..");
			path = Path.Combine(dir, fileName);

			if (File.Exists(path))
				return path;
		}

		return null;
	}

	public static string GetDirectory(string filePath)
	{
		if (string.IsNullOrEmpty(filePath)) return null;

		int index = filePath.LastIndexOfAny(new char[] { '/', '\\' });
		if (index < 0) return string.Empty;

		return filePath.Substring(0, index);
	}

	public static string GetRelativePath(string baseDir, string filePath)
	{
		string relativeDir = DirectoryEx.GetRelativePath(baseDir, FileEx.GetDirectory(filePath));
		return Path.Combine(relativeDir, Path.GetFileName(filePath));
	}

	public static void MoveTo(string targetDir, params string[] files)
	{
		DirectoryEx.Create(targetDir, false);

		foreach (var fromPath in files)
		{
			if (string.IsNullOrWhiteSpace(fromPath)) continue;

			var fileName = Path.GetFileName(fromPath);
			var toPath = Path.Combine(targetDir, fileName);

			File.Delete(toPath);

			try
			{
				File.Move(fromPath, toPath);
			}
			catch (IOException)
			{
				continue;
			}
		}
	}
}
