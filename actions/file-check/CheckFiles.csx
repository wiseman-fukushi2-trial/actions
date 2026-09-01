#nullable enable

using System.Text.RegularExpressions;

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count < 2)
{
	throw new("required branchName file1 [file2 ...]");
}

string branchName = Args[0];
Version expectedVersion = ConvertBranchNameToVersion(branchName);

IEnumerable<string> files = Args.Skip(1);

bool isValid = true;
Dictionary<string, Dictionary<string, ValidationResult>> validation_file_result = [];
foreach (string file in files)
{
	ValidationResult result_ValidateAssemblyFileVersion =
			ValidateAssemblyFileVersion(file, expectedVersion);
	AddResult(validation_file_result, "AssemblyFileVersion", file, result_ValidateAssemblyFileVersion);
}

OutputSummary(validation_file_result);

return isValid ? 0 : -1;

static ValidationResult ValidateAssemblyFileVersion(string content, Version expectedVersion)
{
	if (Path.GetFileName(path) != "AssemblyInfo.vb")
	{
		return ValidationResult.None;
	}

	const string regStr_AssemblyFileVersion = @"<Assembly:\s*AssemblyFileVersion\(""([^""]+)""\)>";
	Match match = Regex.Match(content, regStr_AssemblyFileVersion);

	if (match.Success == false)
	{
		return ValidationResult.Failure;
	}
	string versionStr = match.Groups[1].Value;
	Version version = new(versionStr);
	if (version.Major != expectedVersion.Major ||
	   version.Minor != expectedVersion.Minor ||
	   version.Build != expectedVersion.Build)
	{
		return ValidationResult.Failure;
	}
	if (version.Revision != expectedVersion.Revision)
	{
		return ValidationResult.Failure;
	}
	return ValidationResult.Success;
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

static void AddResult(
	Dictionary<string, Dictionary<string, ValidationResult>> validation_file_result,
	string validationName,
	string file,
	ValidationResult result)
{
	if (validation_file_result.ContainsKey(validationName) == false)
	{
		validation_file_result[validationName] = [];
	}
	validation_file_result[validationName][file] = result;
}

static void OutputSummary(Dictionary<string, Dictionary<string, ValidationResult>> validation_file_result)
{
	List<string> summary = ["## ファイルチェック"];
	foreach (KeyValuePair<string, Dictionary<string, ValidationResult>> line in validation_file_result)
	{
		summary.Add($"### {line.Key}");
		foreach (KeyValuePair<string, ValidationResult> result in line.Value)
		{
			summary.Add($"#### {result.Key}: {result.Value}");
		}
	}

	// 出力
	string summaryFile = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY") ?? "GITHUB_STEP_SUMMARY.log";
	File.AppendAllText(summaryFile, string.Join(Environment.NewLine, summary) + Environment.NewLine);
}

enum ValidationResult
{
	Success,
	Failure,
	Warning,
	None,
}
