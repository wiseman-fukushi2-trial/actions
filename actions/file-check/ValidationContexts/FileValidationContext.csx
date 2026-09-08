#load "./IValidationContext.csx"

public record FileValidationContext : IValidationContext
{
	public string RepositoryRoot { get; }
	public string FullPath { get; }
	public string RelativePath { get; }
	public string FileName { get; }
	public string Content { get; }

	public FileValidationContext(string path, string repositoryRoot)
		: this(path, repositoryRoot, "")
	{
		Content = File.Exists(FullPath) ? File.ReadAllText(FullPath) : "";
	}

	public FileValidationContext(string path, string repositoryRoot, string content)
	{
		string fullPath;
		string relativePath;

		if (Path.IsPathRooted(repositoryRoot) == false)
		{
			throw new ArgumentException($"無効なパス : {repositoryRoot}");
		}

		if (Path.IsPathRooted(path))
		{
			fullPath = Path.GetFullPath(path);
		}
		else
		{
			fullPath =
				Path.GetFullPath(
					Path.Combine(repositoryRoot, path)
				);
		}

		relativePath = Path.GetRelativePath(repositoryRoot, fullPath);
		if (relativePath.StartsWith(".."))
		{
			throw new ArgumentException($"無効なパス : {path}");
		}

		RepositoryRoot = repositoryRoot;
		FullPath = fullPath;
		RelativePath = relativePath;
		FileName = Path.GetFileName(fullPath);
		Content = content;
	}
}
