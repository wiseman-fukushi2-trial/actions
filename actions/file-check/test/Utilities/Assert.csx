using System.Runtime.CompilerServices;

public static class Assert
{
	/// <summary>
	/// 期待される値と実際の値が等しいかどうかを検証する。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected">期待される値</param>
	/// <param name="actual">実際の値</param>
	/// <param name="filePath">呼び出し元のファイルパス（指定がなければ自動取得）</param>
	/// <param name="memberName">呼び出し元のメンバー名（指定がなければ自動取得）</param>
	/// <exception cref="Exception">期待される値と実際の値が等しくない場合にスローされる例外</exception>
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
