public sealed class TempFile : IDisposable
{
	public string Path { get; }

	public TempFile(string fileName)
	{
		Path = Path.Combine(Path.GetTempPath(), fileName);
	}

	public void Dispose()
	{
		if (File.Exists(Path))
		{
			File.Delete(Path);
		}
	}
}
