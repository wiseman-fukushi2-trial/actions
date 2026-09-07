using SysPath = System.IO.Path;

public sealed class TempFile : IDisposable
{
	public string Path { get; }

	public TempFile(string fileName, string content)
	{
		if (SysPath.IsPathRooted(path)) // 絶対パスの場合
		{
			Path = fileName;
		}
		else
		{
			Path = SysPath.Combine(SysPath.GetTempPath(), fileName); // 相対パスの場合
		}
		File.WriteAllText(Path, content);
	}

	public TempFile(string basePath, string fileName, string content)
		: this(SysPath.Combine(basePath, fileName), content) { }

	public void Dispose()
	{
		if (File.Exists(Path))
		{
			File.Delete(Path);
		}
	}
}
