#nullable enable

#load "./Definitions.csx"
#load "./Utility.csx"

using System.Text.RegularExpressions;
using static Definitions;

static class Validation
{
	/// <summary>
	/// AssemblyFileVersion
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

		// AssemblyFileVersion の値を取得
		// 指定されていない場合、または2つ以上指定されている場合はエラーとする
		List<string> versionStrs = Utility.GetAssemblyAttributeValue(path, "AssemblyFileVersion");
		if (versionStrs.Count == 0)
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyFileVersion が見つかりません");
		}
		else if(versionStrs.Count > 1)
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyFileVersion が複数見つかりました");
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

	/// <summary>
	/// AssemblyVersion
	/// </summary>
	/// <param name="path">AssemblyInfo.vb のパス</param>
	/// <param name="rootDir">ソリューションのルートディレクトリ</param>
	/// <remarks>
	/// 基本的には 8.0.0.0
	/// 一部プロジェクトは 8.1.0.0
	/// </remarks>
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
			//{ "CMKCommonSUK", new Version(8, 1, 0, 0) },
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

		// AssemblyVersion の値を取得
		// 指定されていない場合、または2つ以上指定されている場合はエラーとする
		List<string> versionStrs = Utility.GetAssemblyAttributeValue(path, "AssemblyVersion");
		if (versionStrs.Count == 0)
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyVersion が見つかりません");
		}
		else if(versionStrs.Count > 1)
		{
			return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyVersion が複数見つかりました");
		}

		// 期待されるバージョンと比較
		Version version = new(versionStrs[0]);

		if (expectedVersion != version)
		{
			return new ValidationResult(
				path, validationName, ValidationStatus.Failure,
				$"AssemblyVersion {version} が期待されるバージョン {expectedVersion} と一致しません"
			);
		}

		return new ValidationResult(path, validationName, ValidationStatus.Success);
	}

	/// <summary>
	/// ElTabelle
	/// </summary>
	/// <param name="path">ファイルパス</param>
	/// <remarks>
	/// 古いバージョンの ElTabelle モジュールが含まれていないかを検証する
	/// コメントアウト等の考慮はしない
	/// </remarks>
	public static ValidationResult ElTabelle(string path)
	{
		const string validationName = "ElTabelle";
		List<string> targetExtensions = [".vbproj", ".licx", ".resx"];

		List<string> eliminateRegStrs = [
			@"GrapeCity\.Win\.BaseGrid\.v40,.*Version=4\.0\.2006\.224",
			@"GrapeCity\.Win\.WorkBook\.v40,.*Version=4\.0\.2006\.224",
			@"GrapeCity\.Win\.BaseGrid\.v40,.*Version=4\.0\.2007\.1225",
			@"GrapeCity\.Win\.WorkBook\.v40,.*Version=4\.0\.2007\.1225",
		];

		if (targetExtensions.Contains(Path.GetExtension(path)) == false)
		{
			return new ValidationResult(path, validationName, ValidationStatus.None);
		}

		string content = File.ReadAllText(path);
		List<string> foundItems = [];
		foreach (string regStr in eliminateRegStrs)
		{
			Match match = Regex.Match(content, regStr);
			if (match.Success)
			{
				foundItems.Add(match.Value);
			}
		}
		
		if (foundItems.Count > 0) {
			return new ValidationResult(
				path, validationName, ValidationStatus.Failure,
				$"古いバージョンの ElTabelle モジュールが見つかりました: {string.Join(", ", foundItems)}"
			);
		}

		return new ValidationResult(path, validationName, ValidationStatus.Success);
	}

	/// <summary>
	/// bin ディレクトリ
	/// </summary>
	/// <param name="rootDir">ルートディレクトリ</param>
	public static ValidationResult Bin(string rootDir)
	{
		const string validationName = "bin";
		string[] binDirs = Directory.GetDirectories(rootDir, "bin", SearchOption.AllDirectories);
		if(binDirs.Length > 0)
		{
			return new ValidationResult(
				rootDir, validationName, ValidationStatus.Failure,
				$"bin ディレクトリが存在します: {string.Join(", ", binDirs)}"
			);
		}
		return new ValidationResult(rootDir, validationName, ValidationStatus.Success);
	}

	/// <summary>
	/// obj ディレクトリ
	/// </summary>
	/// <param name="rootDir">ルートディレクトリ</param>
	public static ValidationResult Obj(string rootDir)
	{
		const string validationName = "obj";
		string[] objDirs = Directory.GetDirectories(rootDir, "obj", SearchOption.AllDirectories);
		if (objDirs.Length > 0)
		{
			return new ValidationResult(
				rootDir, validationName, ValidationStatus.Failure,
				$"obj ディレクトリが存在します: {string.Join(", ", objDirs)}"
			);
		}
		return new ValidationResult(rootDir, validationName, ValidationStatus.Success);
	}
}
