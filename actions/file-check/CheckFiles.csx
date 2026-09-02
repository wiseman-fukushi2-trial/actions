#nullable enable

using System.Web;
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

List<ValidationResult> results = [];

foreach (string file in files)
{
	results.Add(
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

	results.Add(
		ValidateAssemblyFileVersion(assemblyInfoFile, expectedVersion)
	);
}

OutputSummary(results, rootDir, branchName);

bool hasFailures = results.Any(x => x.Status == ValidationStatus.Failure);

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

static void OutputSummary(List<ValidationResult> results, string rootDir, string branchName)
{
	List<string> summary = ["## ファイルチェック"];

	Dictionary<string, Dictionary<ValidationStatus, List<ValidationResult>>> project_status_results = [];
	foreach (ValidationResult result in results)
	{
		string relativePath = Path.GetRelativePath(rootDir, result.File);
		string projectName = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries)[0];
		project_status_results.TryAdd(projectName, []);
		project_status_results[projectName].TryAdd(result.Status, []);
		project_status_results[projectName][result.Status].Add(result);
	}

	foreach (var project_items in project_status_results)
	{
		string projectName = project_items.Key;
		Dictionary<ValidationStatus, List<ValidationResult>> status_results = project_items.Value;

		ValidationStatus statusForProject = ValidationStatus.None;
		List<string> summaryForProject = [];

		// ValidationStatus の逆順で表示
		foreach (ValidationStatus status in Enum.GetValues<ValidationStatus>().Reverse())
		{
			if(status == ValidationStatus.None)
			{
				//continue;
			}

			if (status_results.TryGetValue(status, out List<ValidationResult>? resultsForStatus) == false)
			{
				continue;
			}

			List<string> summaryForStatus = [];

			foreach (ValidationResult result in resultsForStatus)
			{
				string relativePath = Path.GetRelativePath(rootDir, result.File);

				string encodedPath = HttpUtility.UrlEncode(relativePath);
				string encodedUrl = HttpUtility.UrlEncode($"blob/{branchName}/{encodedPath}");

				summaryForStatus.Add($"#### {result.ValidationName}");
				summaryForStatus.Add($"[{encodedPath}]({encodedUrl})");

				if (result.Status > statusForProject)
				{
					statusForProject = result.Status;
				}
			}
			if (summaryForStatus.Count > 0)
			{
				summaryForProject.Add("<details>");
				summaryForProject.Add($"<summary>{status}</summary>");
				summaryForProject.Add("");
				summaryForProject.AddRange(summaryForStatus);
				summaryForProject.Add("");
				summaryForProject.Add("</details>");
			}
		}

		summary.Add("");
		summary.Add($"### {ValidationStatus_Icon[statusForProject]} {projectName}");
		summary.AddRange(summaryForProject);
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
