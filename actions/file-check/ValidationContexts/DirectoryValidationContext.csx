#load "./IValidationContext.csx"

public record DirectoryValidationContext : IValidationContext
{
	public string RepositoryRoot { get; }
	public string FullPath { get; }
	public string RelativePath { get; }

	public DirectoryValidationContext(string path, string repositoryRoot)
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
	}
}
