#load "./Definitions.csx"
#load "./ValidationContexts/IValidationContext.csx"

using System.Runtime.CompilerServices;

/// <summary>
/// 検証結果レコード
/// </summary>
/// <param name="File">ファイルパス</param>
/// <param name="Status">検証ステータス</param>
/// <param name="ErrorMessage">エラーメッセージ</param>
/// <param name="MemberName">呼び出し元のメンバー名（指定がなければ自動取得）</param>
public record ValidationResult(
	IValidationContext Context,
	Definitions.ValidationStatus Status,
	string ErrorMessage = "",
	[CallerMemberName] string MemberName = ""
);
