#nullable enable

static class Definitions
{
	/// <summary>
	/// 検証結果レコード
	/// </summary>
	/// <param name="File">ファイルパス</param>
	/// <param name="ValidationName">検証名</param>
	/// <param name="Status">検証ステータス</param>
	public record ValidationResult(
		string File,
		string ValidationName,
		ValidationStatus Status,
		string Message = ""
	);

	/// <summary>
	/// 検証ステータス
	/// </summary>
	/// <remarks>
	/// サマリー表示順の決定ロジックが依存している。
	/// 重要度 高 → 低の順に定義すること。
	/// </remarks>
	public enum ValidationStatus
	{
		/// <summary>
		/// 失敗
		/// </summary>
		Failure,
		/// <summary>
		/// 警告
		/// </summary>
		Warning,
		/// <summary>
		/// 検証成功
		/// </summary>
		Success,
		/// <summary>
		/// 検証スキップ
		/// </summary>
		None,
	}

	/// <summary>
	/// 検証ステータスのアイコン
	/// </summary>
	public static Dictionary<ValidationStatus, string> ValidationStatus_Icon = new()
	{
		{ ValidationStatus.Failure, ":x:" },
		{ ValidationStatus.Warning, ":warning:" },
		{ ValidationStatus.Success, ":white_check_mark:" },
		{ ValidationStatus.None, ":small_blue_diamond:" },
	};
}
