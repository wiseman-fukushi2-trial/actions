#nullable enable

//required [relativePath1 ...]

// 差分ファイルパスのリスト
IList<string> changedFiles = Args;

// 差分プロジェクトのリスト
HashSet<string> changedProjects = [];

foreach (string file in changedFiles)
{
	string? directoryPath = Path.GetDirectoryName(file);

	if (directoryPath == null || Directory.Exists(directoryPath) == false)
	{
		continue;
	}
	DirectoryInfo directory = new(directoryPath);
	string? projFilePath = GetProjFilePath(directory);
	if (projFilePath == null)
	{
		continue;
	}
	changedProjects.Add(projFilePath);
}

string result = string.Join(" ", changedProjects.Select(x => $"'{x}'"));

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? "GITHUB_OUTPUT.log";
File.AppendAllText(outputFile, $"changed_projects={result}" + Environment.NewLine);


static string? GetProjFilePath(DirectoryInfo directory)
{
	FileInfo[] files = directory.GetFiles("*.vbproj");
	if (files.Length > 1)
	{
		throw new($"{directory.FullName} に *.vbproj が複数存在します。");
	}
	if (files.Length == 1)
	{
		return files[0].FullName;
	}
	else
	{
		if (directory.Parent == null)
		{
			return null;
		}
		return GetProjFilePath(directory.Parent);
	}
}
