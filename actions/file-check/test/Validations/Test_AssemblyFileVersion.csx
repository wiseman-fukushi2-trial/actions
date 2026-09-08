#load "../Utilities/Assert.csx"
#load "../../Definitions.csx"
#load "../../Validation.csx"
#load "../../ValidationContexts/FileValidationContext.csx"

using static Definitions;

static class Test_AssemblyFileVersion
{
	public static void Exec()
	{
		Success();
		Failure_Majorの不一致();
		Failure_Minorの不一致();
		Failure_Buildの不一致();
		Warning_Revisionの不一致();
		Failure_指定無し();
		Failure_コメントアウト();
		Failure_複数指定();
		None_ファイル名();
	}

	static string RepoRoot = Path.GetTempPath();

	static void Success()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
		);
		Version version = new(20, 9, 6, 0);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_Majorの不一致()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("19.9.6.0")>
			"""
		);
		Version version = new(20, 9, 6, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_Minorの不一致()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.10.6.0")>
			"""
		);
		Version version = new(20, 9, 6, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_Buildの不一致()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.9.5.0")>
			"""
		);
		Version version = new(20, 9, 6, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Warning_Revisionの不一致()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.9.8.1")>
			"""
		);
		Version version = new(20, 9, 8, 0);

		Assert.AreEqual(
			ValidationStatus.Warning,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_指定無し()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion_("20.9.8.0")>
			"""
		);
		Version version = new(20, 9, 8, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_コメントアウト()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			' <Assembly:AssemblyFileVersion("20.9.8.0")>
			"""
		);
		Version version = new(20, 9, 8, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void Failure_複数指定()
	{
		FileValidationContext context = new(
			"AssemblyInfo.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.9.8.0")>
			<Assembly:AssemblyFileVersion("20.9.8.0")>
			"""
		);
		Version version = new(20, 9, 8, 0);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}

	static void None_ファイル名()
	{
		FileValidationContext context = new(
			"AssemblyInfo_.vb",
			RepoRoot,
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
		);
		Version version = new(20, 9, 6, 0);

		Assert.AreEqual(
			ValidationStatus.None,
			Validation.AssemblyFileVersion(context, version).Status
		);
	}
}
