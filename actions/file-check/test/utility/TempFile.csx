public sealed class TempFile : IDisposable
{
	public string Path { get; }

	public TempFile(string fileName, string content)
	{
		if (System.IO.Path.IsPathRooted(path)) // 絶対パスの場合
		{
			Path = fileName;
		}
		else
		{
			Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName); // 相対パスの場合
		}
		File.WriteAllText(Path, content);
	}

	public TempFile(string basePath, string fileName, string content)
		: this(Path.Combine(basePath, fileName), content) { }

	public void Dispose()
	{
		if (File.Exists(Path))
		{
			File.Delete(Path);
		}
	}
}
