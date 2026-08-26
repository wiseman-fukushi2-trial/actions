"## ビルド結果" >> $env:GITHUB_STEP_SUMMARY

foreach ($vbproj in $args) {
	Write-Host "Building $vbproj"

	$exe = ($vbproj.Contains("front")) ?
		"$env:MSBUILD_2019\MSBuild.exe" :
		"$env:MSBUILD\MSBuild.exe"

	$msBuildArgs = ($vbproj.Contains("front")) ?
		@(
			"$vbproj"
			"/p:TargetFrameworkSDKToolsDirectory=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
			"/v:diag"
		) :
		@(
			"$vbproj"
		)

	& $exe @msBuildArgs
	$exitCode = $LASTEXITCODE

	"### $vbproj" >> $env:GITHUB_STEP_SUMMARY
	if ($exitCode.Equals(0)) {
		Write-Host "Successfully built $vbproj"
		"✅ **成功**" >> $env:GITHUB_STEP_SUMMARY
	}
	else {
		Write-Host "Failed to build $vbproj"
		"❌ **失敗**" >> $env:GITHUB_STEP_SUMMARY
	}

	exit $exitCode
}
