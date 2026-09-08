using System.Runtime.CompilerServices;

public static class Assert
{
	public static void AreEqual<T>(
		T expected, T actual,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		bool result = EqualityComparer<T>.Default.Equals(expected, actual);

		string memberFullName = $"{Path.GetFileNameWithoutExtension(filePath)}.{memberName}";

		Console.WriteLine($"{(result ? "::notice::" : "::error::")}{memberFullName}");
		Console.WriteLine($"{(result ? "::notice::" : "::error::")}  {expected} => {actual}");

		if (result == false)
		{
			throw new Exception($"Assertion failed: Expected {expected}, but got {actual}.");
		}
	}

	public static void IsTrue(
		bool condition,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		AreEqual(true, condition, filePath, memberName);
	}

	public static void IsFalse(
		bool condition,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		AreEqual(false, condition, filePath, memberName);
	}
}
