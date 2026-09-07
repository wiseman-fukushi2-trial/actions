#load "utility/Assert.csx"
#load "../Validation.csx"
#load "../Definitions.csx"

using static Definitions;

static partial class ValidationTest
{
	public static void AssemblyFileVersion()
	{
		正常系();
	}

	static void 正常系()
	{
		using TempFile file = new(
			"AssemblyInfo.vb",
			"""
			<Assembly:AssemblyFileVersion("20.9.6.0")>
			"""
			);
		static Version version = new(20, 9, 6, 0);
		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.AssemblyFileVersion(file, version).Status
		);
	}
}
