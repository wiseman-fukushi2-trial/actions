#nullable enable

#r "nuget: Microsoft.Build, 18.9.6"

using Microsoft.Build.Graph;

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count == 1)
{
	throw new("required {{ github.workspace }} projectFile1 [projectFile2 ...]");
}

// ソリューションファイルを検索
string rootDir = Args[0];

DirectoryInfo directory = new(rootDir);
FileInfo[] solutionFiles = directory.GetFiles("*.sln");

if (solutionFiles.Length == 0)
{
	throw new("solution file not found");
}

else if (solutionFiles.Length > 1)
{
	throw new("multiple solution files found");
}

string solutionFile = solutionFiles[0].FullName;

IEnumerable<string> projectFiles = Args.Skip(1);

ProjectGraph graph = new(Args[0]);

foreach (ProjectGraphNode node in graph.ProjectNodesTopologicallySorted)
{
    Console.WriteLine(node.ProjectInstance.FullPath);
}
