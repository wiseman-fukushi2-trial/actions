#load "../Utilities/Assert.csx"
#load "../Utilities/TempFile.csx"
#load "../Utilities/TempDirectory.csx"
#load "../../Definitions.csx"
#load "../../Validation.csx"
#load "../../ValidationContexts/DirectoryValidationContext.csx"

using static Definitions;

static class Test_Obj
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
		Success_ファイル名がobj();
	}

	static void Success()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(context).Status
		);
	}

	static void Failure()
	{
		using TempDirectory dir = new(@"testSolution\testProject\obj");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(context).Status
		);
	}

	static void Success_ルートディレクトリ()
	{
		using TempDirectory dir = new(@"obj\testProject");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(context).Status
		);
	}

	static void Failure_ルートディレクトリ直下()
	{
		using TempDirectory dir = new(@"testSolution\obj");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(context).Status
		);
	}

	static void Failure_深い階層()
	{
		using TempDirectory dir = new(@"testSolution\testProject\testDir\testDir2\obj");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(context).Status
		);
	}

	static void Failure_UpperCase()
	{
		using TempDirectory dir = new(@"testSolution\testProject\Obj");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(context).Status
		);
	}

	static void Success_類似名()
	{
		using TempDirectory dir = new(@"testSolution\testProject\object");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(context).Status
		);
	}

	static void Success_ファイル名がobj()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");
		using TempFile fileA = new(dir.Root, "obj", "ルート直下");
		using TempFile fileB = new(dir.Path, "obj", "深い階層");
		DirectoryValidationContext context = new(dir.Root);

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(context).Status
		);
	}
}
