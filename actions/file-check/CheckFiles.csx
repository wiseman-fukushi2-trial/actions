#nullable enable

using System.Text.RegularExpressions;
using System.Xml.Linq;

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count < 4)
{
	throw new("required github.workspace branchName file1 [file2 ...] projectFile1 [projectFile2 ...]");
}

string rootDir = Args[0];

string branchName = Args[1];
Version expectedVersion = ConvertBranchNameToVersion(branchName);

IEnumerable<string> files = Args.Skip(2).Where(x => x.EndsWith(".vbproj") == false);
IEnumerable<string> projectFiles = Args.Skip(2).Where(x => x.EndsWith(".vbproj"));

Dictionary<string, List<ValidationResult>> file_results =
	files
	.Select(x => new KeyValuePair<string, List<ValidationResult>>(x, []))
	.ToDictionary(x => x.Key, x => x.Value);

foreach (string file in files)
{
	file_results[file].Add(
		ValidateAssemblyFileVersion(file, expectedVersion)
	);
}

foreach (string projectFile in projectFiles)
{
	IEnumerable<string> assemblyInfoFiles = GetAssemblyInfoPaths(projectFile);
	if (assemblyInfoFiles.Any() == false)
	{
		throw new Exception($"No AssemblyInfo.vb found for {projectFile}");
	}
	else if (assemblyInfoFiles.Count() > 1)
	{
		throw new Exception($"Multiple AssemblyInfo.vb found for {projectFile}");
	}

	string assemblyInfoFile = assemblyInfoFiles.First();
	if (file_results.ContainsKey(assemblyInfoFile) == false)
	{
		file_results[assemblyInfoFile] = [];
	}

	file_results[assemblyInfoFile].Add(
		ValidateAssemblyFileVersion(assemblyInfoFile, expectedVersion)
	);
}

OutputSummary(file_results, rootDir);

bool hasFailures =
	file_results
	.SelectMany(x => x.Value)
	.Any(x => x.Status == ValidationStatus.Failure);

return hasFailures ? -1 : 0;


static ValidationResult ValidateAssemblyFileVersion(string path, Version expectedVersion)
{
	const string validationName = "AssemblyFileVersion";

	if (Path.GetFileName(path) != "AssemblyInfo.vb")
	{
		return new ValidationResult(path, validationName, ValidationStatus.None);
	}

	string content = File.ReadAllText(path);
	const string regStr_AssemblyFileVersion = @"<Assembly:\s*AssemblyFileVersion\(""([^""]+)""\)>";
	Match match = Regex.Match(content, regStr_AssemblyFileVersion);

	if (match.Success == false)
	{
		return new ValidationResult(path, validationName, ValidationStatus.Failure);
	}
	string versionStr = match.Groups[1].Value;
	Version version = new(versionStr);
	if (version.Major != expectedVersion.Major ||
	   version.Minor != expectedVersion.Minor ||
	   version.Build != expectedVersion.Build)
	{
		return new ValidationResult(path, validationName, ValidationStatus.Failure);
	}
	if (version.Revision != expectedVersion.Revision)
	{
		return new ValidationResult(path, validationName, ValidationStatus.Failure);
	}
	return new ValidationResult(path, validationName, ValidationStatus.Success);
}

public static List<string> GetAssemblyInfoPaths(string vbprojPath)
{
	if (Path.GetExtension(vbprojPath) != ".vbproj")
	{
		return [];
	}

	string projectDir = Path.GetDirectoryName(vbprojPath) ?? throw new ArgumentException("vbprojPath is not valid.");

	XDocument doc = XDocument.Load(vbprojPath);

	return doc.Descendants()
		.Where(x => x.Name.LocalName == "Compile")
		.Select(x => x.Attribute("Include")?.Value)
		.Where(x => string.IsNullOrWhiteSpace(x) == false)
		.Where(x => Path.GetFileName(x) == "AssemblyInfo.vb")
		.Select(x => Path.GetFullPath(
			Path.Combine(projectDir, x!)))
		.ToList();
}

static Version ConvertBranchNameToVersion(string branchName)
{
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

static void OutputSummary(Dictionary<string, List<ValidationResult>> file_results, string rootDir)
{
	List<string> summary = ["## ファイルチェック"];
	List<string> sortedKeys = file_results.Keys.ToList();
	sortedKeys.Sort();

	foreach (string file in sortedKeys)
	{
		List<ValidationResult> results = file_results[file];
		// ファイルとしてのステータスを決定する（Failure > Warning > Success > None）
		ValidationStatus representative =
			results
			.Select(x => x.Status)
			.OrderByDescending(x => x)
			.FirstOrDefault();
		summary.Add($"### {ValidationStatus_Icon[representative]} {Path.GetRelativePath(rootDir, file)}");
		foreach (ValidationResult result in results)
		{
			summary.Add($"#### {result.ValidationName}: {result.Status}");
		}
	}

	// 出力
	string summaryFile = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY") ?? "GITHUB_STEP_SUMMARY.log";
	File.AppendAllText(summaryFile, string.Join(Environment.NewLine, summary) + Environment.NewLine);
}

record ValidationResult(
	string File,
	string ValidationName,
	ValidationStatus Status
);

enum ValidationStatus
{
	None,
	Success,
	Warning,
	Failure,
}

static Dictionary<ValidationStatus, string> ValidationStatus_Icon = new()
{
	{ ValidationStatus.Success, ":white_check_mark:" },
	{ ValidationStatus.Failure, ":x:" },
	{ ValidationStatus.Warning, ":warning:" },
	{ ValidationStatus.None, ":small_blue_diamond:" },
};
