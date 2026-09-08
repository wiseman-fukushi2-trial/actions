#load "../Utilities/Assert.csx"
#load "../../Definitions.csx"
#load "../../Validation.csx"
#load "../../ValidationContexts/FileValidationContext.csx"

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

	static string RepoRoot = Path.GetTempPath();

	static void Success()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			' <Assembly: AssemblyVersion("1.0.*")>
			<Assembly: AssemblyVersion("8.0.0.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_Major不一致()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion("9.0.0.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_Minor不一致()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion("8.1.0.0")>
			"""
			);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_Build不一致()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion("8.0.1.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_Revision不一致()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion("8.0.0.1")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
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
			FileValidationContext context = new(
				@$"{project}\My Project\AssemblyInfo.vb",
				RepoRoot,
				"""
				<Assembly: AssemblyVersion("8.1.0.0")>
				"""
			);

			Assert.AreEqual(
				ValidationStatus.Success,
				Validation.AssemblyVersion(context).Status,
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
			FileValidationContext context = new(
				@$"{project}\My Project\AssemblyInfo.vb",
				RepoRoot,
				"""
				<Assembly: AssemblyVersion("8.0.0.0")>
				"""
			);

			Assert.AreEqual(
				ValidationStatus.Failure,
				Validation.AssemblyVersion(context).Status,
				memberName: $"{nameof(Failure_特殊)}_{project}"
			);
		}
	}

	static void Failure_指定無し()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion_("8.0.0.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_コメントアウト()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			' <Assembly: AssemblyVersion("8.0.0.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void Failure_複数指定()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly: AssemblyVersion("8.0.0.0")>
			<Assembly: AssemblyVersion("8.0.0.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyVersion(context).Status
		);
	}

	static void None_ファイル名()
	{
		FileValidationContext context = new(
			@"testProject\My Project\AssemblyInfo_.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyVersion("20.9.6.0")>
			"""
		);

		Assert.AreEqual(
			ValidationStatus.None,
			Validation.AssemblyVersion(context).Status
		);
	}
}
