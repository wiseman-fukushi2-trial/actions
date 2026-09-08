#load "../Utilities/Assert.csx"
#load "../Utilities/TempFile.csx"
#load "../../Validation.csx"
#load "../../Definitions.csx"

using static Definitions;

static class Test_ElTabelle
{
	public static void Exec()
	{
		Success_licx();
		Success_resx();
		Success_vbproj();
		Failure_licx();
		Failure_resx();
		Failure_vbproj();
		None_拡張子();
	}

	static string RepoRoot = Path.GetTempPath();

	static void Success_licx()
	{
		FileValidationContext context = new(
			"licenses.licx",
			RepoRoot,
			"""
			GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.WorkBook.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7
			GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.BaseGrid.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.ElTabelle(context).Status
		);
	}

	static void Success_resx()
	{
		FileValidationContext context = new(
			"Resources.resx",
			RepoRoot,
			"""
			<?xml version="1.0" encoding="utf-8"?>
			<root>
			  <resheader name="reader">
			    <value>GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.WorkBook.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7</value>
			  </resheader>
			  <resheader name="writer">
			    <value>GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.BaseGrid.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7</value>
			  </resheader>
			</root>
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.ElTabelle(context).Status
		);
	}

	static void Success_vbproj()
	{
		FileValidationContext context = new(
			"TestProject.vbproj",
			RepoRoot,
			"""
			<?xml version="1.0" encoding="utf-8"?>
			<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="Current">
			  <ItemGroup>
			    <Reference Include="GrapeCity.Win.WorkBook.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7, processorArchitecture=MSIL">
			      <SpecificVersion>False</SpecificVersion>
			      <HintPath>C:\WisemanVSystem\Application\CommonLibrary\GrapeCity.Win.WorkBook.v40.dll</HintPath>
			      <Private>False</Private>
			    </Reference>
			    <Reference Include="GrapeCity.Win.BaseGrid.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7, processorArchitecture=MSIL">
			      <SpecificVersion>False</SpecificVersion>
			      <HintPath>C:\WisemanVSystem\Application\CommonLibrary\GrapeCity.Win.BaseGrid.v40.dll</HintPath>
			      <Private>False</Private>
			    </Reference>
			  </ItemGroup>
			</Project>
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Success,
			Validation.ElTabelle(context).Status
		);
	}

	static void Failure_licx()
	{
		FileValidationContext context = new(
			"licenses.licx",
			RepoRoot,
			"""
			GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.WorkBook.v40, Version=4.0.2006.224, Culture=neutral, PublicKeyToken=abc123def456ghi7
			GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.BaseGrid.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.ElTabelle(context).Status
		);
	}

	static void Failure_resx()
	{
		FileValidationContext context = new(
			"Resources.resx",
			RepoRoot,
			"""
			<?xml version="1.0" encoding="utf-8"?>
			<root>
			  <resheader name="reader">
			    <value>GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.WorkBook.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7</value>
			  </resheader>
			  <resheader name="writer">
			    <value>GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.BaseGrid.v40, Version=4.0.2006.224, Culture=neutral, PublicKeyToken=abc123def456ghi7</value>
			  </resheader>
			</root>
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.ElTabelle(context).Status
		);
	}

	static void Failure_vbproj()
	{
		FileValidationContext context = new(
			"TestProject.vbproj",
			RepoRoot,
			"""
			<?xml version="1.0" encoding="utf-8"?>
			<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="Current">
			  <ItemGroup>
			    <Reference Include="GrapeCity.Win.WorkBook.v40, Version=4.0.2007.1225, Culture=neutral, PublicKeyToken=abc123def456ghi7, processorArchitecture=MSIL">
			      <SpecificVersion>False</SpecificVersion>
			      <HintPath>C:\WisemanVSystem\Application\CommonLibrary\GrapeCity.Win.WorkBook.v40.dll</HintPath>
			      <Private>False</Private>
			    </Reference>
			    <Reference Include="GrapeCity.Win.BaseGrid.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7, processorArchitecture=MSIL">
			      <SpecificVersion>False</SpecificVersion>
			      <HintPath>C:\WisemanVSystem\Application\CommonLibrary\GrapeCity.Win.BaseGrid.v40.dll</HintPath>
			      <Private>False</Private>
			    </Reference>
			  </ItemGroup>
			</Project>
			"""
		);
		Assert.AreEqual(
			ValidationStatus.Failure,
			Validation.ElTabelle(context).Status
		);
	}

	static void None_拡張子()
	{
		FileValidationContext context = new(
			"licenses.vb",
			RepoRoot,
			"""
			GrapeCity.Win.ElTabelle.Sheet, GrapeCity.Win.WorkBook.v40, Version=4.0.2008.1215, Culture=neutral, PublicKeyToken=abc123def456ghi7
			"""
		);
		Assert.AreEqual(
			ValidationStatus.None,
			Validation.ElTabelle(context).Status
		);
	}
}
