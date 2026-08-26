function AddSummary {
    param(
        [string[]]$Text
    )

    $Text | Add-Content $env:GITHUB_STEP_SUMMARY
}

AddSummary @("## ビルド結果")

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
			"/v:quiet"
			"/nologo"
		) :
		@(
			"$vbproj"
			"/v:quiet"
			"/nologo"
		)

	$output = & $exe @msBuildArgs 2>&1

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

	$errors = $output | Where-Object { $_ -match ':\s*error\s' }
	$warnings = $output | Where-Object { $_ -match ':\s*warning\s' }

	if ($errors) {
		AddSummary @(
			"<details>"
			"<summary>❌ Errors ($($errors.Count))</summary>"
			"<pre>"
			$($errors -join "`n")
			"</pre>"
			"</details>"
			""
		)
		Write-Host "Errors:"
		$errors | ForEach-Object { Write-Host $_ }
	}
	if ($warnings) {
		AddSummary @(
			"<details>"
			"<summary>⚠ Warnings ($($warnings.Count))</summary>"
			"<pre>"
			$($warnings -join "`n")
			"</pre>"
			"</details>"
			""
		)
		Write-Host "Warnings:"
		$warnings | ForEach-Object { Write-Host $_ }
	}
}

exit $exitCode
