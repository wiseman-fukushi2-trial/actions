#nullable enable


// 引数が空の場合は処理を終了する
if (Args is null || Args.Count == 0)
{
	throw new("required {{ github.workspace }} relativePath1 relativePath2 ...");
}
else if (Args.Count == 1)
{
	Console.WriteLine("差分ファイルがありません");
	return;
}

string root = Args[0];

// 差分ファイルパスのリスト
IEnumerable<string> relativePaths = Args.Skip(1);

// 差分プロジェクトのリスト
IEnumerable<string> absolutePaths = relativePaths.Select(relative => Path.Combine(root, relative));

string result = string.Join(' ', absolutePaths);

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? "GITHUB_OUTPUT.log";
File.AppendAllText(outputFile,$"changed_projects={string.Join(" ", absolutePaths)}" + Environment.NewLine);


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
