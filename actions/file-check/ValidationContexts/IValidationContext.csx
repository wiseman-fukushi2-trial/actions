public interface IValidationContext
{
	string RepositoryRoot { get; }
	string FullPath { get; }
	string RelativePath { get; }
}
