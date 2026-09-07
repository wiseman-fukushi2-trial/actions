#load "../utility/Assert.csx"
#load "../utility/TempFile.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static partial class ValidationTest
{
	public static void AssemblyFileVersion()
	{
		正常系();
		異常系();
	}

	static void 正常系()
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

	static void 異常系()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
			);
		Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.AssemblyFileVersion(file.Path, version).Status
		);
	}
}
