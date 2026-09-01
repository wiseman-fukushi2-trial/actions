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
foreach (string file in files)
{
	isValid &= ValidateAssemblyFileVersion(file, expectedVersion);
}

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? "GITHUB_OUTPUT.log";
File.AppendAllText(outputFile,$"changed_files={string.Join(" ", absolutePaths)}" + Environment.NewLine);

return isValid ? 0 : -1;

static bool ValidateAssemblyFileVersion(string content, Version expectedVersion)
{
	const string regStr_AssemblyFileVersion = @"<Assembly:\s*AssemblyFileVersion\(""([^""]+)""\)>";
	Match match = Regex.Match(content, regStr_AssemblyFileVersion);

	if (match.Success == false)
	{
		return false;
	}
	string versionStr = match.Groups[1].Value;
	Version version = new(versionStr);
	if (version.Major != expectedVersion.Major ||
	   version.Minor != expectedVersion.Minor ||
	   version.Build != expectedVersion.Build)
	{
		return false;
	}
	if (version.Revision != expectedVersion.Revision)
	{
		return false;
	}
	return version == expectedVersion;
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
