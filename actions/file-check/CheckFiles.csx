#nullable enable

#r "nuget: Microsoft.CodeAnalysis.VisualBasic, 5.9.0"

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml.Linq;

#region メイン

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count < 6)
{
	throw new("required workspace repositoryUrl branchName sha file1 [file2 ...] projectFile1 [projectFile2 ...]");
}

// ルートディレクトリ
string rootDir = Args[0];
// リポジトリの URL
string repositoryUrl = Args[1];
// ブランチ名
string branchName = Args[2];
// コミット SHA
string sha = Args[3];
// 変更されたファイル
IEnumerable<string> files = Args.Skip(4).Where(x => x.EndsWith(".vbproj") == false);
// プロジェクトファイル
IEnumerable<string> projectFiles = Args.Skip(4).Where(x => x.EndsWith(".vbproj"));

// 期待されるバージョンをブランチ名から取得する
Version expectedVersion = ConvertBranchNameToVersion(branchName);

List<ValidationResult> results = [];

foreach (string file in files)
{
	results.AddRange([
		Validate_AssemblyFileVersion(file, expectedVersion),
		Validate_AssemblyVersion(file, rootDir),
	]);
}

foreach (string projectFile in projectFiles)
{
	// AssemblyInfo.vb のパス
	IEnumerable<string> assemblyInfoFiles = GetAssemblyInfoPaths(projectFile);
	// AssemblyInfo.vb が指定されていない場合、または2つ以上指定されている場合はエラーとする
	if (assemblyInfoFiles.Any() == false)
	{
		throw new Exception($"No AssemblyInfo.vb found for {projectFile}");
	}
	else if (assemblyInfoFiles.Count() > 1)
	{
		throw new Exception($"Multiple AssemblyInfo.vb found for {projectFile}");
	}

	string assemblyInfoFile = assemblyInfoFiles.First();

	results.AddRange([
		Validate_AssemblyFileVersion(assemblyInfoFile, expectedVersion),
		Validate_AssemblyVersion(assemblyInfoFile, rootDir),
	]);
}

OutputSummary(results, rootDir, repositoryUrl, sha);

// 失敗が1件でもあれば、終了コード -1 を返す
bool hasFailures = results.Any(x => x.Status == ValidationStatus.Failure);

return hasFailures ? -1 : 0;

#endregion

#region バリデーション

/// <summary>
/// チェック AssemblyFileVersion
/// </summary>
/// <param name="path">AssemblyInfo.vb のパス</param>
/// <param name="expectedVersion">期待されるバージョン</param>
/// <remarks>
/// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
/// </remarks>
static ValidationResult Validate_AssemblyFileVersion(string path, Version expectedVersion)
{
	const string validationName = "AssemblyFileVersion";

	if (Path.GetFileName(path) != "AssemblyInfo.vb")
	{
		return new ValidationResult(path, validationName, ValidationStatus.None);
	}

	string content = File.ReadAllText(path);

	// <Assembly: AssemblyFileVersion("00.0.0.00")>
	const string regStr_AssemblyFileVersion = @"<Assembly:\s*AssemblyFileVersion\(""([^""]+)""\)>";
	Match match = Regex.Match(content, regStr_AssemblyFileVersion);

	// AssemblyFileVersion が見つからない場合は失敗とする
	if (match.Success == false)
	{
		return new ValidationResult(path, validationName, ValidationStatus.Failure, "AssemblyFileVersion が見つかりません");
	}

	// 期待されるバージョンと比較
	// Major, Minor, Build が一致しない場合は失敗とする
	// Revision が一致しない場合は警告とする（標準化資料に、Revision はインクリメントするという記載あり）
	string versionStr = match.Groups[1].Value;
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

static ValidationResult Validate_AssemblyVersion(string path, string rootDir)
{
	const string validationName = "AssemblyVersion";
	// 基本的には 8.0.0.0
	// 一部プロジェクト は8.1.0.0
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

	if (Path.GetFileName(path) != "AssemblyInfo.vb")
	{
		return new ValidationResult(path, validationName, ValidationStatus.None);
	}

	string content = File.ReadAllText(path);

	// <Assembly: AssemblyVersion("0.0.0.0")>
	const string regStr_AssemblyVersion = @"<Assembly:\s*AssemblyVersion\(""([^""]+)""\)>";
	Match match = Regex.Match(content, regStr_AssemblyVersion);

	// AssemblyVersion が見つからない場合は失敗とする
	if (match.Success == false)
	{
		return new ValidationResult(path, validationName, ValidationStatus.Failure);
	}

	// ソリューションルートからの相対パスで、先頭のディレクトリ名をプロジェクト名とする
	string relativePath = Path.GetRelativePath(rootDir, path);
	string projectName = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];
	Version expectedVersion =
		specialProject_versions.TryGetValue(projectName, out Version? specialVersion)
		? specialVersion : defaultVersion;

	// 期待されるバージョンと比較
	//string versionStr = match.Groups[1].Value;
	string versionStr = GetAssemblyAttributeValue(path, "AssemblyVersion") ?? throw new Exception("null value");
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

#endregion

#region ヘルパー
/// <summary>
/// vbproj ファイルから AssemblyInfo.vb のパスを取得する
/// </summary>
/// <param name="vbprojPath">vbproj ファイルのパス</param>
/// <returns>AssemblyInfo.vb のパスのリスト</returns>
public static List<string> GetAssemblyInfoPaths(string vbprojPath)
{
	if (Path.GetExtension(vbprojPath) != ".vbproj")
	{
		return [];
	}

	// プロジェクトファイルが配置されているディレクトリをルートとする
	string projectDir = Path.GetDirectoryName(vbprojPath) ?? throw new ArgumentException("vbprojPath is not valid.");

	XDocument doc = XDocument.Load(vbprojPath);

	// <Compile Include="My Project\AssemblyInfo.vb" />
	return doc.Descendants()
		.Where(x => x.Name.LocalName == "Compile")
		.Select(x => x.Attribute("Include")?.Value)
		.Where(x => string.IsNullOrWhiteSpace(x) == false)
		.Where(x => Path.GetFileName(x) == "AssemblyInfo.vb")
		// ルートディレクトリと相対パスを結合する
		.Select(x => Path.GetFullPath(Path.Combine(projectDir, x!)))
		// 複数定義される可能性があるため、リストとして返す
		.ToList();
}

/// <summary>
/// ブランチ名からバージョンを取得する
/// </summary>
/// <param name="branchName">ブランチ名</param>
/// <returns>バージョン</returns>
/// <exception cref="ArgumentException">ブランチ名の先頭が feature-000000 の形式から始まらない場合</exception>
static Version ConvertBranchNameToVersion(string branchName)
{
	// feature-<Major><Minor><Build><Revision>
	const string regStr_BranchNameToVersion = @"^DX-(\d{2})(\d{1})(\d{1})(\d{2})";
	Match match = Regex.Match(branchName, regStr_BranchNameToVersion);
	if (match.Success == false)
	{
		throw new ArgumentException("The branch name does not match the expected pattern.");
	}
	return new(
		int.Parse(match.Groups[1].Value),
		int.Parse(match.Groups[2].Value),
		int.Parse(match.Groups[3].Value),
		int.Parse(match.Groups[4].Value)
		);
}

static string? GetAssemblyAttributeValue(
	string assemblyInfoPath,
	string attributeName)
{
	return "10.5.5";
	/*
	string source = File.ReadAllText(assemblyInfoPath);

	SyntaxTree tree = VisualBasicSyntaxTree.ParseText(source);

	SyntaxNode root = tree.GetRoot();

	IEnumerable<AttributeSyntax> attributes = root
		.DescendantNodes()
		.OfType<AttributeSyntax>();

	foreach (AttributeSyntax attribute in attributes)
	{
		string name = attribute.Name.ToString();

		if (!string.Equals(
				name,
				attributeName,
				StringComparison.OrdinalIgnoreCase))
		{
			continue;
		}

		ArgumentSyntax? arg = attribute.ArgumentList?
			.Arguments
			.FirstOrDefault();

		if (arg?.GetExpression() is LiteralExpressionSyntax literal)
		{
			return literal.Token.ValueText;
		}
	}

	return null;
	*/
}

/// <summary>
/// 結果を GitHub Actions の Summary に出力する
/// </summary>
/// <param name="results">ValidationResult のリスト</param>
/// <param name="rootDir">ソリューションのルートディレクトリ</param>
/// <param name="repositoryUrl">リポジトリの URL</param>
/// <param name="sha">SHA</param>
static void OutputSummary(List<ValidationResult> results, string rootDir, string repositoryUrl, string sha)
{
	// 全体のサマリー
	List<string> summary = ["## ファイルチェック"];

	// { プロジェクト名, { 検証ステータス, [ 結果レコード ] } }
	Dictionary<string, Dictionary<ValidationStatus, List<ValidationResult>>> project_status_results = [];
	foreach (ValidationResult result in results)
	{
		// ソリューションルートからの相対パスで、先頭のディレクトリ名をプロジェクト名とする
		string relativePath = Path.GetRelativePath(rootDir, result.File);
		string projectName = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];

		project_status_results.TryAdd(projectName, []);
		project_status_results[projectName].TryAdd(result.Status, []);
		project_status_results[projectName][result.Status].Add(result);
	}

	foreach (var project_items in project_status_results)
	{
		// プロジェクト名
		string projectName = project_items.Key;
		// { 検証ステータス, [結果レコード] }
		Dictionary<ValidationStatus, List<ValidationResult>> status_results = project_items.Value;

		// プロジェクト単位の検証ステータス
		ValidationStatus statusForProject = ValidationStatus.None;
		// プロジェクト単位のサマリー
		List<string> summaryForProject = [];

		// ValidationStatus の逆順で表示
		foreach (ValidationStatus status in Enum.GetValues<ValidationStatus>().Reverse())
		{
			// 検証をスキップした場合は表示しない
			if (status == ValidationStatus.None)
			{
				continue;
			}

			// 検証結果が存在しない場合はスキップ
			if (status_results.TryGetValue(status, out List<ValidationResult>? resultsForStatus) == false)
			{
				continue;
			}

			// 検証ステータス単位のサマリー
			List<string> summaryForStatus = [];

			foreach (ValidationResult result in resultsForStatus)
			{
				string relativePath = Path.GetRelativePath(rootDir, result.File);

				string path = relativePath.Replace(" ", "");
				string url = $"{repositoryUrl}/blob/{sha}/{relativePath}".Replace("\\", "/").Replace(" ", "%20");

				summaryForStatus.Add($"#### {result.ValidationName}");
				if(string.IsNullOrWhiteSpace(result.Message) == false)
				{
					summaryForStatus.Add($"<sub>{result.Message}</sub>");
				}
				summaryForStatus.Add($"[{path}]({url})");

				// プロジェクト単位の検証ステータスを更新
				if (result.Status > statusForProject)
				{
					statusForProject = result.Status;
				}
			}
			if (summaryForStatus.Count > 0)
			{
				// 検証ステータス単位のサマリーを折りたたみ表示する
				summaryForProject.Add("<details>");
				summaryForProject.Add($"<summary>{status}</summary>");
				summaryForProject.Add("");
				summaryForProject.AddRange(summaryForStatus);
				summaryForProject.Add("");
				summaryForProject.Add("</details>");
			}
		}

		// プロジェクト単位のサマリーを追加
		summary.Add("");
		summary.Add($"### {ValidationStatus_Icon[statusForProject]} {projectName}");
		summary.AddRange(summaryForProject);
	}

	// 出力
	string summaryFile = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY") ?? "GITHUB_STEP_SUMMARY.log";
	File.AppendAllText(summaryFile, string.Join(Environment.NewLine, summary) + Environment.NewLine);
}

#endregion

#region 定義

/// <summary>
/// 検証結果レコード
/// </summary>
/// <param name="File">ファイルパス</param>
/// <param name="ValidationName">検証名</param>
/// <param name="Status">検証ステータス</param>
record ValidationResult(
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
/// 重要度 低 → 高の順に定義すること。
/// </remarks>
enum ValidationStatus
{
	/// <summary>
	/// 検証スキップ
	/// </summary>
	None,
	/// <summary>
	/// 検証成功
	/// </summary>
	Success,
	/// <summary>
	/// 警告
	/// </summary>
	Warning,
	/// <summary>
	/// 失敗
	/// </summary>
	Failure,
}

/// <summary>
/// 検証ステータスのアイコン
/// </summary>
static Dictionary<ValidationStatus, string> ValidationStatus_Icon = new()
{
	{ ValidationStatus.None, ":small_blue_diamond:" },
	{ ValidationStatus.Success, ":white_check_mark:" },
	{ ValidationStatus.Warning, ":warning:" },
	{ ValidationStatus.Failure, ":x:" },
};

#endregion
