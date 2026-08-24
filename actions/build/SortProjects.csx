#r "Microsoft.Build"
#r "Microsoft.Build.Framework"
#r "Microsoft.Build.Utilities.Core"

#nullable enable

using Microsoft.Build.Graph;

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count == 0)
{
	throw new("required solutionFile projectFile1 [projectFile2 ...]");
}

string solutionFile = Args[0];

IEnumerable<string> projectFiles = Args.Skip(1);

ProjectGraph graph = new(args[0]);

foreach (var node in graph.ProjectNodesTopologicallySorted)
{
    Console.WriteLine(node.ProjectInstance.FullPath);
}
