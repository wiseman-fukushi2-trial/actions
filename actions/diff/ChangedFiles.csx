#nullable enable

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count < 1)
{
	throw new("required github.workspace [changedFile1 ...]");
}

string root = Args[0];

// 差分ファイルパスのリスト
IEnumerable<string> relativePaths = Args.Skip(1);

// 差分プロジェクトのリスト
IEnumerable<string> absolutePaths = relativePaths.Select(relative => Path.Combine(root, relative));

if (absolutePaths.Any() == false)
{
	return;
}

string result = string.Join(" ", absolutePaths.Select(x => $"'{x}'"));

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? "GITHUB_OUTPUT.log";
File.AppendAllText(outputFile, $"changed_files={result}" + Environment.NewLine);
