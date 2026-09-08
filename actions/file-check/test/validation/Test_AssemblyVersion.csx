#load "../utility/Assert.csx"
#load "../utility/TempFile.csx"
#load "../utility/TempDirectory.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static class Test_AssemblyVersion
{
	public static void Exec()
	{
		Success();
		Failure_Major不一致();
		Failure_Minor不一致();
		Failure_Build不一致();
		Failure_Revision不一致();
		Success_特殊();
		Failure_特殊();
		Failure_指定無し();
		Failure_コメントアウト();
		Failure_複数指定();
		None_ファイル名();
	}

	static void Success()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			' <Assembly: AssemblyVersion("1.0.*")>
			<Assembly: AssemblyVersion("8.0.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_Major不一致()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion("9.0.0.0")>
			"""
			);
		
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_Minor不一致()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion("8.1.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_Build不一致()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion("8.0.1.0")>
			"""
			);
		
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_Revision不一致()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion("8.0.0.1")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Success_特殊()
	{
		List<string> projects_8100 = [
			"CMKCommonSUK",
			"CMKControlSUK",
			"CMKFieldSUK",
			"CMKFormSUK",
			"CMKGmnSUK",
			"CMKManagerSUK",
			"CMKPrintSUK",
			"CMKTableSUK",
			"CMKTableExtSUK",
		];

		foreach (string project in projects_8100)
		{
			using TempDirectory dir = new(@$"testSolution\{project}\My Project");
			using TempFile file = new(
				dir.Path,
				"AssemblyInfo.vb",
				"""
				<Assembly: AssemblyVersion("8.1.0.0")>
				"""
				);

			Assert.AreEqual(
				ValidationStatus.Success,
				Validation.AssemblyVersion(file.Path, dir.Root).Status,
				memberName: $"{nameof(Success_特殊)}_{project}"
			);
		}
	}

	static void Failure_特殊()
	{
		List<string> projects_8100 = [
			"CMKCommonSUK",
			"CMKControlSUK",
			"CMKFieldSUK",
			"CMKFormSUK",
			"CMKGmnSUK",
			"CMKManagerSUK",
			"CMKPrintSUK",
			"CMKTableSUK",
			"CMKTableExtSUK",
		];

		foreach (string project in projects_8100)
		{
			using TempDirectory dir = new(@$"testSolution\{project}\My Project");
			using TempFile file = new(
				dir.Path,
				"AssemblyInfo.vb",
				"""
				<Assembly: AssemblyVersion("8.0.0.0")>
				"""
				);

			Assert.AreEqual(
				ValidationStatus.Failure,
				Validation.AssemblyVersion(file.Path, dir.Root).Status,
				memberName: $"{nameof(Failure_特殊)}_{project}"
			);
		}
	}

	static void Failure_指定無し()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion_("8.0.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_コメントアウト()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			' <Assembly: AssemblyVersion("8.0.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void Failure_複数指定()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo.vb",
			"""
			<Assembly: AssemblyVersion("8.0.0.0")>
			<Assembly: AssemblyVersion("8.0.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}

	static void None_ファイル名()
	{
		using TempDirectory dir = new(@"testSolution\testProject\My Project");
		using TempFile file = new(
			dir.Path,
			"AssemblyInfo_.vb",
			"""
			<Assembly:AssemblyVersion("20.9.6.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.None,
			Validation.AssemblyVersion(file.Path, dir.Root).Status
		);
	}
}
