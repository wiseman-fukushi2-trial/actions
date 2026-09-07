using SysPath = System.IO.Path;

public sealed class TempDirectory : IDisposable
{
	public string Root { get; }
	public string Path { get; }

	public TempDirectory(string path)
	{
		string tempDir = SysPath.GetFullPath(SysPath.GetTempPath());

		string fullPath;
		if (SysPath.IsPathRooted(path))
		{
			fullPath = SysPath.GetFullPath(path);
		}
		else
		{
			fullPath =
				SysPath.GetFullPath(
					SysPath.Combine(tempDir, path)
				);
		}

		string relativePath = SysPath.GetRelativePath(tempDir, fullPath);
		if (relativePath == "." || relativePath.StartWith(".."))
		{
			throw new ArgumentException($"無効なパス : {path}");
		}

		Path = fullPath;
		Root =
			SysPath.GetFullPath(
				SysPath.Combine(
					tempDir,
					relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0]
				)
			);

		// ディレクトリの作成
		if (Directory.Exists(Path) == false)
		{
			Directory.CreateDirectory(Path);
		}
	}

	public TempDirectory(string basePath, string path)
		: this(Path.Combine(basePath, path)) { }

	public void Dispose()
	{
		if (Directory.Exists(Root))
		{
			Directory.Delete(Root, true);
		}
	}
}
