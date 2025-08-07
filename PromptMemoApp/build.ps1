Write-Host "Building PromptMemoApp..." -ForegroundColor Green

try {
    $result = & msbuild PromptMemoApp.csproj /p:Configuration=Debug /verbosity:minimal
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build successful!" -ForegroundColor Green
    } else {
        Write-Host "Build failed with error code $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "Error running MSBuild: $_" -ForegroundColor Red
}

Read-Host "Press Enter to continue"
