#load "../utility/Assert.csx"
#load "../utility/TempFile.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static partial class ValidationTest
{
	public static void AssemblyFileVersion()
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

	static void Success()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_Majorの不一致()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("19.9.6.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_Minorの不一致()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.10.6.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_Buildの不一致()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.5.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Warning_Revisionの不一致()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.8.1")>
			"""
			);
		Version version = new(20, 9, 8, 0);
		Assert.AreEqual(
			ValidationStatus.Warning,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_指定無し()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion_("20.9.8.0")>
			"""
			);
		Version version = new(20, 9, 8, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_コメントアウト()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			' <Assembly:AssemblyFileVersion("20.9.8.0")>
			"""
			);
		Version version = new(20, 9, 8, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void Failure_複数指定()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.8.0")>
			<Assembly:AssemblyFileVersion("20.9.8.0")>
			"""
			);
		Version version = new(20, 9, 8, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}

	static void None_ファイル名()
	{
		using TempFile file = new(
			"AssemblyInfo_.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.None,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}
}
