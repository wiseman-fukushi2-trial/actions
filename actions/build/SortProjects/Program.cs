using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
				if (args.Length < 2)
				{
					throw new ArgumentException("required solutionFilePath projectFile1 [projectFile2 ...]");
				}

				string workspace = args[0];
				IEnumerable<string> projectFiles = args.Skip(1);

				string solutionFile = Directory
					.GetFiles(workspace, "*.sln", SearchOption.TopDirectoryOnly)
					.Single();

				string msbuildPath = Environment.GetEnvironmentVariable("MSBUILD") ?? throw new Exception("MSBUILD environment variable not set");

				MSBuildLocator.RegisterMSBuildPath(msbuildPath);

				Console.WriteLine($"MSBuild registered: {MSBuildLocator.IsRegistered}");

				List<string> buildingOrder = GetBuildingOrder(solutionFile).ToList();
				Console.WriteLine("Original order:");
				foreach (var project in buildingOrder)
				{
					Console.WriteLine(project);
				}

				IEnumerable<string> sortedProjects = projectFiles.OrderBy(x => buildingOrder.IndexOf(x));
				Console.WriteLine("Sorted order:");
				foreach (var project in sortedProjects)
				{
					Console.WriteLine(project);
				}

				string result = string.Join(" ", sortedProjects);

				// 出力
				string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? "GITHUB_OUTPUT.log";
				File.AppendAllText(outputFile, $"sorted_projects={result}" + Environment.NewLine);
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
		static IEnumerable<string> GetBuildingOrder(string solutionFile)
		{
			var graph = new ProjectGraph(solutionFile);
			return graph.ProjectNodesTopologicallySorted.Select(x => x.ProjectInstance.FullPath);
		}
	}
}
