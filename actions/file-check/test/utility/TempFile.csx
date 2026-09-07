public sealed class TempFile : IDisposable
{
	public string Path { get; }

	public TempFile(string fileName, string content)
	{
		Path = Path.Combine(Path.GetTempPath(), fileName);
		File.WriteAllText(Path, content);
	}

	public void Dispose()
	{
		if (File.Exists(Path))
		{
			File.Delete(Path);
		}
	}
}
