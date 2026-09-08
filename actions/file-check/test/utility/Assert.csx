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
		string displayMessage = "";
		displayMessage += $"{Path.GetFileNameWithoutExtension(filePath)}.{memberName} ";
		displayMessage += $"{expected} => {actual} ";
		displayMessage += result == false ? message : "";
		Console.WriteLine((result ? "::notice::" : "::error::") + displayMessage);
		if (result == false)
		{
			throw new Exception(displayMessage);
		}
	}
	public static void IsTrue(bool condition, string message = "")
	{
		if (!condition)
		{
			throw new Exception($"Assertion failed: Condition is not true. {message}");
		}
	}
	public static void IsFalse(bool condition, string message = "")
	{
		if (condition)
		{
			throw new Exception($"Assertion failed: Condition is not false. {message}");
		}
	}
}
