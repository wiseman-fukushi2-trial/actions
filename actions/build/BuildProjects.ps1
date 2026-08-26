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
}
