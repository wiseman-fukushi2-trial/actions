#load "../utility/Assert.csx"
#load "../utility/TempFile.csx"
#load "../utility/TempDirectory.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static class Test_Obj
{
	public static void Exec()
	{
		Success();
		Failure();
		Failure_ルートディレクトリ();
		Failure_ルートディレクトリ直下();
		Failure_深い階層();
		Failure_UpperCase();
		Success_類似名();
		Success_ファイル名がobj();
	}

	static void Success()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Failure()
	{
		using TempDirectory dir = new(@"testSolution\testProject\obj");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Failure_ルートディレクトリ()
	{
		using TempDirectory dir = new(@"obj\testProject");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Failure_ルートディレクトリ直下()
	{
		using TempDirectory dir = new(@"testSolution\obj");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Failure_深い階層()
	{
		using TempDirectory dir = new(@"testSolution\testProject\testDir\testDir2\obj");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Failure_UpperCase()
	{
		using TempDirectory dir = new(@"testSolution\testProject\Obj");

		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Success_類似名()
	{
		using TempDirectory dir = new(@"testSolution\testProject\object");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(dir.Root).Status
		);
	}

	static void Success_ファイル名がobj()
	{
		using TempDirectory dir = new(@"testSolution\testProject\test");
		using TempFile fileA = new(dir.Root, "obj", "ルート直下");
		using TempFile fileB = new(dir.Path, "obj", "深い階層");

		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.Obj(dir.Root).Status
		);
	}
}
