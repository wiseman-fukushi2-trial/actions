using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Build.Graph;
using Microsoft.Build.Locator;

namespace SortProjects
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
					throw new ArgumentException("required solutionFile [projectFile1 projectFile2 ...]");
				}

				string workspace = args[0];

				string solutionFile = Directory
					.GetFiles(workspace, "*.sln", SearchOption.TopDirectoryOnly)
					.Single();

				string msbuildPath = Environment.GetEnvironmentVariable("MSBUILD") ?? throw new Exception("MSBUILD environment variable not set");

				MSBuildLocator.RegisterMSBuildPath(msbuildPath);

				Console.WriteLine($"MSBuild registered: {MSBuildLocator.IsRegistered}");

				RunProjectGraph(solutionFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Exception:");

				Console.WriteLine(ex.GetType().FullName);
				Console.WriteLine(ex.Message);

				if (ex.InnerException != null)
				{
					Console.WriteLine();
					Console.WriteLine("InnerException:");

					Console.WriteLine(ex.InnerException.GetType().FullName);
					Console.WriteLine(ex.InnerException.Message);
				}

				throw;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void RunProjectGraph(string solutionFile)
		{
			var graph = new ProjectGraph(solutionFile);

			foreach (ProjectGraphNode node in graph.ProjectNodesTopologicallySorted)
			{
				Console.WriteLine(node.ProjectInstance.FullPath);
			}
		}
	}
}
