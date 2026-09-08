#nullable enable

#load "./Definitions.csx"
#load "./Utility.csx"
#load "./ValidationContexts/DirectoryValidationContext.csx"
#load "./ValidationContexts/FileValidationContext.csx"
#load "./ValidationResult.csx"

using System.Text.RegularExpressions;
using static Definitions;

static class Validation
{
	/// <summary>
	/// AssemblyFileVersion
	/// </summary>
	/// <param name="context">検証コンテキスト</param>
	/// <param name="expectedVersion">期待されるバージョン</param>
	/// <remarks>
	/// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
	/// </remarks>
	public static ValidationResult AssemblyFileVersion(FileValidationContext context, Version expectedVersion)
	{
		if (context.FileName != "AssemblyInfo.vb")
		{
			return new ValidationResult(context, ValidationStatus.None);
		}

		// AssemblyFileVersion の値を取得
		// 指定されていない場合、または2つ以上指定されている場合はエラーとする
		List<string> versionStrs = Utility.GetAssemblyAttributeValue(context.Content, "AssemblyFileVersion");
		if (versionStrs.Count == 0)
		{
			return new ValidationResult(context, ValidationStatus.Failure, "AssemblyFileVersion が見つかりません");
		}
		else if(versionStrs.Count > 1)
		{
			return new ValidationResult(context, ValidationStatus.Failure, "AssemblyFileVersion が複数見つかりました");
		}

		// 期待されるバージョンと比較
		// Major, Minor, Build が一致しない場合は失敗とする
		// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
		Version version = new(versionStrs[0]);
		if (version.Major != expectedVersion.Major ||
		   version.Minor != expectedVersion.Minor ||
		   version.Build != expectedVersion.Build)
		{
			return new ValidationResult(
				context, ValidationStatus.Failure,
				$"AssemblyFileVersion {version} が期待されるバージョン {expectedVersion} と一致しません"
			);
		}
		if (version.Revision != expectedVersion.Revision)
		{
			return new ValidationResult(
				context, ValidationStatus.Warning,
				$"AssemblyFileVersion {version} の Revision が期待されるバージョン {expectedVersion} と一致しません"
			);
		}
		return new ValidationResult(context, ValidationStatus.Success);
	}

	/// <summary>
	/// AssemblyVersion
	/// </summary>
	/// <param name="context">検証コンテキスト</param>
	/// <remarks>
	/// 基本的には 8.0.0.0
	/// 一部プロジェクトは 8.1.0.0
	/// </remarks>
	public static ValidationResult AssemblyVersion(FileValidationContext context)
	{
		if (Path.GetFileName(context.FileName) != "AssemblyInfo.vb")
		{
			return new ValidationResult(context, ValidationStatus.None);
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
			{ "CMKTableExtSUK", new Version(8, 1, 0, 0) },
		};

		// ソリューションルートからの相対パスで、先頭のディレクトリ名をプロジェクト名とする
		string projectName = context.RelativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];
		Version expectedVersion =
			specialProject_versions.TryGetValue(projectName, out Version? specialVersion)
			? specialVersion : defaultVersion;

		// AssemblyVersion の値を取得
		// 指定されていない場合、または2つ以上指定されている場合はエラーとする
		List<string> versionStrs = Utility.GetAssemblyAttributeValue(context.Content, "AssemblyVersion");
		if (versionStrs.Count == 0)
		{
			return new ValidationResult(context, ValidationStatus.Failure, "AssemblyVersion が見つかりません");
		}
		else if(versionStrs.Count > 1)
		{
			return new ValidationResult(context, ValidationStatus.Failure, "AssemblyVersion が複数見つかりました");
		}

		// 期待されるバージョンと比較
		Version version = new(versionStrs[0]);

		if (version != expectedVersion)
		{
			return new ValidationResult(
				context, ValidationStatus.Failure,
				$"AssemblyVersion {version} が期待されるバージョン {expectedVersion} と一致しません"
			);
		}

		return new ValidationResult(context, ValidationStatus.Success);
	}

	/// <summary>
	/// ElTabelle
	/// </summary>
	/// <param name="context">検証コンテキスト</param>
	/// <remarks>
	/// 古いバージョンの ElTabelle モジュールが含まれていないかを検証する
	/// コメントアウト等の考慮はしない
	/// </remarks>
	public static ValidationResult ElTabelle(FileValidationContext context)
	{
		List<string> targetExtensions = [".vbproj", ".licx", ".resx"];

		List<string> eliminateRegStrs = [
			@"GrapeCity\.Win\.BaseGrid\.v40,.*Version=4\.0\.2006\.224",
			@"GrapeCity\.Win\.WorkBook\.v40,.*Version=4\.0\.2006\.224",
			@"GrapeCity\.Win\.BaseGrid\.v40,.*Version=4\.0\.2007\.1225",
			@"GrapeCity\.Win\.WorkBook\.v40,.*Version=4\.0\.2007\.1225",
		];

		if (targetExtensions.Contains(Path.GetExtension(context.FileName)) == false)
		{
			return new ValidationResult(context, ValidationStatus.None);
		}

		List<string> foundItems = [];
		foreach (string regStr in eliminateRegStrs)
		{
			Match match = Regex.Match(context.Content, regStr);
			if (match.Success)
			{
				foundItems.Add(match.Value);
			}
		}
		
		if (foundItems.Count > 0) {
			return new ValidationResult(
				context, ValidationStatus.Failure,
				$"古いバージョンの ElTabelle モジュールが見つかりました: {string.Join(", ", foundItems)}"
			);
		}

		return new ValidationResult(context, ValidationStatus.Success);
	}

	/// <summary>
	/// bin ディレクトリ
	/// </summary>
	/// <param name="context">検証コンテキスト</param>
	public static ValidationResult Bin(DirectoryValidationContext context)
	{
		string[] binDirs = Directory.GetDirectories(context.RepositoryRoot, "bin", SearchOption.AllDirectories);
		if(binDirs.Length > 0)
		{
			return new ValidationResult(
				context, ValidationStatus.Failure,
				$"bin ディレクトリが存在します: {string.Join(", ", binDirs)}"
			);
		}
		return new ValidationResult(context, ValidationStatus.Success);
	}

	/// <summary>
	/// obj ディレクトリ
	/// </summary>
	/// <param name="context">検証コンテキスト</param>
	public static ValidationResult Obj(DirectoryValidationContext context)
	{
		string[] objDirs = Directory.GetDirectories(context.RepositoryRoot, "obj", SearchOption.AllDirectories);
		if (objDirs.Length > 0)
		{
			return new ValidationResult(
				context, ValidationStatus.Failure,
				$"obj ディレクトリが存在します: {string.Join(", ", objDirs)}"
			);
		}
		return new ValidationResult(context, ValidationStatus.Success);
	}
}
