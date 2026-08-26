"## ビルド結果" >> $env:GITHUB_STEP_SUMMARY

$exitCode = 0

foreach ($vbproj in $args) {
	Write-Host "Building $vbproj"

	$exe = ($vbproj.Contains("front")) ?
		"$env:MSBUILD_2019\MSBuild.exe" :
		"$env:MSBUILD\MSBuild.exe"

	$msBuildArgs = ($vbproj.Contains("front")) ?
		@(
			"$vbproj"
			"/p:TargetFrameworkSDKToolsDirectory=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
			"/clp:ErrorsOnly"
			"/nologo"
		) :
		@(
			"$vbproj"
			"/clp:ErrorsOnly"
			"/nologo"
		)

	& $exe @msBuildArgs

	"### $([System.IO.Path]::GetFileNameWithoutExtension($vbproj))" >> $env:GITHUB_STEP_SUMMARY
	if ($LASTEXITCODE.Equals(0)) {
		Write-Host "Successfully built $vbproj"
		"✅ 成功" >> $env:GITHUB_STEP_SUMMARY
	}
	else {
		$exitCode = $LASTEXITCODE
		Write-Host "Failed to build $vbproj"
		"❌ 失敗" >> $env:GITHUB_STEP_SUMMARY
	}
}

exit $exitCode
