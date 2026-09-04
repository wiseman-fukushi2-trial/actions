#nullable enable

#load "./Definitions.csx"
#load "./Validation.csx"
#load "./Utility.csx"

#r "nuget: Microsoft.CodeAnalysis.VisualBasic, 4.14.0"

using System.Xml.Linq;
using static Definitions;

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
Version expectedVersion = Utility.ConvertBranchNameToVersion(branchName);

List<ValidationResult> results = [];

foreach (string file in files)
{
	results.AddRange([
		Validation.AssemblyFileVersion(file, expectedVersion),
		Validation.AssemblyVersion(file, rootDir),
	]);
}

foreach (string projectFile in projectFiles)
{
	// AssemblyInfo.vb のパス
	IEnumerable<string> assemblyInfoFiles = Utility.GetAssemblyInfoPaths(projectFile);
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
		Validation.AssemblyFileVersion(assemblyInfoFile, expectedVersion),
		Validation.AssemblyVersion(assemblyInfoFile, rootDir),
	]);
}

Utility.OutputSummary(results, rootDir, repositoryUrl, sha);

// 失敗が1件でもあれば、終了コード -1 を返す
bool hasFailures = results.Any(x => x.Status == ValidationStatus.Failure);

return hasFailures ? -1 : 0;
