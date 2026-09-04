#nullable enable

#load "./Definitions.csx"
#load "./Utility.csx"

using static Definitions;

static class Validation
{
	/// <summary>
	/// チェック AssemblyFileVersion
	/// </summary>
	/// <param name="path">AssemblyInfo.vb のパス</param>
	/// <param name="expectedVersion">期待されるバージョン</param>
	/// <remarks>
	/// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
	/// </remarks>
	public static ValidationResult AssemblyFileVersion(string path, Version expectedVersion)
	{
		const string validationName = "AssemblyFileVersion";

		if (Path.GetFileName(path) != "AssemblyInfo.vb")
		{
			return new ValidationResult(path, validationName, ValidationStatus.None);
		}

		// 期待されるバージョンと比較
		// Major, Minor, Build が一致しない場合は失敗とする
		// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
		string? versionStr = Utility.GetAssemblyAttributeValue(path, "AssemblyFileVersion");
		if (string.IsNullOrEmpty(versionStr))
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyFileVersion が見つかりません");
		}

		Version version = new(versionStr);
		if (version.Major != expectedVersion.Major ||
		   version.Minor != expectedVersion.Minor ||
		   version.Build != expectedVersion.Build)
		{
			return new ValidationResult(
				path, validationName, ValidationStatus.Failure,
				$"AssemblyFileVersion {version} が期待されるバージョン {expectedVersion} と一致しません"
			);
		}
		if (version.Revision != expectedVersion.Revision)
		{
			return new ValidationResult(
				path, validationName, ValidationStatus.Warning,
				$"AssemblyFileVersion {version} の Revision が期待されるバージョン {expectedVersion} と一致しません"
			);
		}
		return new ValidationResult(path, validationName, ValidationStatus.Success);
	}

	public static ValidationResult AssemblyVersion(string path, string rootDir)
	{
		const string validationName = "AssemblyVersion";

		if (Path.GetFileName(path) != "AssemblyInfo.vb")
		{
			return new ValidationResult(path, validationName, ValidationStatus.None);
		}

		// 基本的には 8.0.0.0
		// 一部プロジェクト は 8.1.0.0
		Version defaultVersion = new(8, 0, 0, 0);
		Dictionary<string, Version> specialProject_versions = new()
		{
			{ "CMKCommonSUK", new Version(8, 1, 0, 0) },
			{ "CMKControlSUK", new Version(8, 1, 0, 0) },
			{ "CMKFieldSUK", new Version(8, 1, 0, 0) },
			{ "CMKFormSUK", new Version(8, 1, 0, 0) },
			{ "CMKGmnSUK", new Version(8, 1, 0, 0) },
			{ "CMKManagerSUK", new Version(8, 1, 0, 0) },
			{ "CMKPrintSUK", new Version(8, 1, 0, 0) },
			{ "CMKTableSUK", new Version(8, 1, 0, 0) },
			{ "CMKTableExtSUK", new Version(8, 0, 0, 0) },
		};

		// ソリューションルートからの相対パスで、先頭のディレクトリ名をプロジェクト名とする
		string relativePath = Path.GetRelativePath(rootDir, path);
		string projectName = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];
		Version expectedVersion =
			specialProject_versions.TryGetValue(projectName, out Version? specialVersion)
			? specialVersion : defaultVersion;

		// 期待されるバージョンと比較
		string? versionStr = Utility.GetAssemblyAttributeValue(path, "AssemblyVersion");
		if (string.IsNullOrEmpty(versionStr))
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyVersion が見つかりません");
		}

		Version version = new(versionStr);

		if (expectedVersion != version)
		{
			return new ValidationResult(
				path, validationName, ValidationStatus.Failure,
				$"AssemblyVersion {version} が期待されるバージョン {expectedVersion} と一致しません"
			);
		}

		return new ValidationResult(path, validationName, ValidationStatus.Success);
	}
}
