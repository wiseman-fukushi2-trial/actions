foreach ($vbproj in $args) {
	Write-Host "Building $vbproj"

	if ($vbproj.Contains("front")) {
		& "$env:MSBUILD_2019\MSBuild.exe" "$vbproj"
	}
	else {
		& "$env:MSBUILD\MSBuild.exe" "$vbproj"
	}
}
