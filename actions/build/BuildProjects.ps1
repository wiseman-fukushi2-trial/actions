foreach ($vbproj in $args) {
	Write-Host "Building $vbproj"
	if ($vbproj.Contains("front")) {
		& "%MSBUILD_2019%\MSBuild.exe" "$vbproj"
	}
	else {
		& "%MSBUILD%\MSBuild.exe" "$vbproj"
	}
}
