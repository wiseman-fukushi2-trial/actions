public static class Assert
{
	public static void AreEqual<T>(T expected, T actual, string message = "")
	{
		if (!EqualityComparer<T>.Default.Equals(expected, actual))
		{
			throw new Exception($"Assertion failed: Expected {expected}, but got {actual}. {message}");
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
