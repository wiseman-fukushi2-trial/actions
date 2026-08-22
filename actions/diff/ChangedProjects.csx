// 引数が空の場合は処理を終了する
if(Args is null || Args.Count == 0)
{
	Console.WriteLine("差分ファイルがありません。");
	return;
}

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
	changedProjects.Add(projectName);
}

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
File.AppendAllText(outputFile,$"changed_projects={string.Join(" ", changedProjects)}" + Environment.NewLine);


static string? GetProjFilePath(DirectoryInfo directory)
{
	FileInfo[] files = directory.GetFiles(".vbproj");
	if (files.Length > 1)
	{
		throw new($"{directory.FullName} に .vbproj が複数存在します。");
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
