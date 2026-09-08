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
