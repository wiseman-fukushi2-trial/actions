#nullable enable

#load "./Definitions.csx"

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using static Definitions;

static class Utility
{
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
	public static Version ConvertBranchNameToVersion(string branchName)
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

	public static string? GetAssemblyAttributeValue(string assemblyInfoPath, string attributeName)
	{
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
	}

	/// <summary>
	/// 結果を GitHub Actions の Summary に出力する
	/// </summary>
	/// <param name="results">ValidationResult のリスト</param>
	/// <param name="rootDir">ソリューションのルートディレクトリ</param>
	/// <param name="repositoryUrl">リポジトリの URL</param>
	/// <param name="sha">SHA</param>
	public static void OutputSummary(List<ValidationResult> resultItems, string rootDir, string repositoryUrl, string sha)
	{
		// 全体のサマリー
		List<string> summary = ["## ファイルチェック"];

		// { プロジェクト名, { 検証ステータス, { 検証名, [ 結果レコード ] } } }
		Dictionary<string, Dictionary<ValidationStatus, Dictionary<string, List<ValidationResult>>>> project_status_validation_results = [];
		foreach (ValidationResult result in resultItems)
		{
			// ソリューションルートからの相対パスで、先頭のディレクトリ名をプロジェクト名とする
			string relativePath = Path.GetRelativePath(rootDir, result.File);
			string projectName = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];

			project_status_validation_results.TryAdd(projectName, []);
			project_status_validation_results[projectName].TryAdd(result.Status, []);
			project_status_validation_results[projectName][result.Status].TryAdd(result.ValidationName, []);
			project_status_validation_results[projectName][result.Status][result.ValidationName].Add(result);
		}

		foreach (var project_items in project_status_validation_results)
		{
			// プロジェクト名
			string projectName = project_items.Key;
			// { 検証ステータス, { 検証名, [ 結果レコード ] } }
			Dictionary<ValidationStatus, Dictionary<string, List<ValidationResult>>>
				status_validation_results = project_items.Value;

			// プロジェクト単位の検証ステータス
			// 全体の内、最も小さい値を優先する
			ValidationStatus statusForProject = ValidationStatus.None;
			// プロジェクト単位のサマリー
			List<string> summaryForProject = [];

			// ValidationStatus の順で表示
			foreach (ValidationStatus status in Enum.GetValues<ValidationStatus>())
			{
				// 検証をスキップした場合は表示しない
				if (status == ValidationStatus.None)
				{
					continue;
				}

				// 検証結果が存在しない場合はスキップ
				if (status_validation_results.TryGetValue(status, out Dictionary<string, List<ValidationResult>>? validation_results) == false)
				{
					continue;
				}

				// 検証ステータス単位のサマリー
				List<string> summaryForStatus = [];

				foreach (var validation_items in validation_results)
				{
					string validationName = validation_items.Key;
					List<ValidationResult> results = validation_items.Value;

					summaryForStatus.Add($"#### {validationName}");

					foreach (ValidationResult result in results)
					{
						string relativePath = Path.GetRelativePath(rootDir, result.File);

						string path = relativePath.Replace(" ", "");
						string url = $"{repositoryUrl}/blob/{sha}/{relativePath}".Replace("\\", "/").Replace(" ", "%20");

						if (string.IsNullOrWhiteSpace(result.Message) == false)
						{
							summaryForStatus.Add($"<sub>{result.Message}</sub>");
						}
						summaryForStatus.Add($"[{path}]({url})");

						// プロジェクト単位の検証ステータスを更新
						if (result.Status < statusForProject)
						{
							statusForProject = result.Status;
						}
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
}
