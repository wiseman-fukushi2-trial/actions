using System.Runtime.CompilerServices;

public static class Assert
{
	public static void AreEqual<T>(
		T expected, T actual,
		string message = "",
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		bool result = EqualityComparer<T>.Default.Equals(expected, actual);

		string memberFullName = $"{Path.GetFileNameWithoutExtension(filePath)}.{memberName}";
		if (result)
		{
			Console.WriteLine($"::notice::{memberFullName}");
			Console.WriteLine($"::notice::{expected} => {actual}");
		}
		if (result == false)
		{
			Console.WriteLine($"::error::{memberFullName}");
			Console.WriteLine($"::error::{expected} => {actual}");
			Console.WriteLine($"::error::{message}");
		}
		if (result == false)
		{
			throw new Exception($"Assertion failed: Expected {expected}, but got {actual}. {message}");
		}
	}

	public static void IsTrue(
		bool condition,
		string message = "",
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		AreEqual(true, condition, message, filePath, memberName);
	}

	public static void IsFalse(
		bool condition,
		string message = "",
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "")
	{
		AreEqual(false, condition, message, filePath, memberName);
	}
}
