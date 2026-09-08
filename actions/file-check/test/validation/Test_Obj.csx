#load "../utility/Assert.csx"
#load "../utility/TempFile.csx"
#load "../utility/TempDirectory.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static class Test_Bin
{
	public static void Exec()
	{
		Success();
		Failure();
		Success_ルートディレクトリ();
		Failure_ルートディレクトリ直下();
		Failure_深い階層();
		Failure_UpperCase();
		Success_類似名();
		Success_ファイル名がbin();
	}

	static void Success()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Failure()
	{
		using TempDirectory dir = new(@"testSolution\testProject\bin");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Success_ルートディレクトリ()
	{
		using TempDirectory dir = new(@"bin\testProject");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Failure_ルートディレクトリ直下()
	{
		using TempDirectory dir = new(@"testSolution\bin");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Failure_深い階層()
	{
		using TempDirectory dir = new(@"testSolution\testProject\testDir\testDir2\bin");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Failure_UpperCase()
	{
		using TempDirectory dir = new(@"testSolution\testProject\Bin");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Success_類似名()
	{
		using TempDirectory dir = new(@"testSolution\testProject\binary");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(dir.Root).Status
		);
	}

	static void Success_ファイル名がbin()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");
		using TempFile fileA = new(dir.Root, "bin", "ルート直下");
		using TempFile fileB = new(dir.Path, "bin", "深い階層");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(dir.Root).Status
		);
	}
}
