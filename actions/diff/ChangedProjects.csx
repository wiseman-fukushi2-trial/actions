// 引数が空の場合は処理を終了する
if(Args is null || Args.Count == 0)
{
	Console.WriteLine("差分ファイルがありません。");
	return;
}

// 差分ファイルパスのリスト
IList<string> changedFiles = Args;

// 差分プロジェクト名のリスト
HashSet<string> changedProjects = [];

foreach (string file in changedFiles)
{
	// ルート直下のファイルは無視する
	if(file.Contains('/') == false)
	{
		continue;
	}
	// プロジェクトルートのディレクトリ名をプロジェクト名として扱う
	string projectName = file.Split('/')[0];
	changedProjects.Add(projectName);
}

// 出力
string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
File.AppendAllText(outputFile,$"changed_projects={string.Join(" ", changedProjects)}" + Environment.NewLine);
