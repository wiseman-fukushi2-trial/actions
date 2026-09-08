#load "../Utilities/Assert.csx"
#load "../Utilities/TempFile.csx"
#load "../Utilities/TempDirectory.csx"
#load "../../Definitions.csx"
#load "../../Validation.csx"
#load "../../ValidationContexts/DirectoryValidationContext.csx"

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
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(context).Status
		);
	}

	static void Failure()
	{
		using TempDirectory dir = new(@"testSolution\testProject\bin");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(context).Status
		);
	}

	static void Success_ルートディレクトリ()
	{
		using TempDirectory dir = new(@"bin\testProject");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(context).Status
		);
	}

	static void Failure_ルートディレクトリ直下()
	{
		using TempDirectory dir = new(@"testSolution\bin");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(context).Status
		);
	}

	static void Failure_深い階層()
	{
		using TempDirectory dir = new(@"testSolution\testProject\testDir\testDir2\bin");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(context).Status
		);
	}

	static void Failure_UpperCase()
	{
		using TempDirectory dir = new(@"testSolution\testProject\Bin");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Bin(context).Status
		);
	}

	static void Success_類似名()
	{
		using TempDirectory dir = new(@"testSolution\testProject\binary");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(context).Status
		);
	}

	static void Success_ファイル名がbin()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");
		using TempFile fileA = new(dir.Root, "bin", "ルート直下");
		using TempFile fileB = new(dir.Path, "bin", "深い階層");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Bin(context).Status
		);
	}
}
