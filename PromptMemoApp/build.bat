@echo off
echo Building PromptMemoApp...
msbuild PromptMemoApp.csproj /p:Configuration=Debug /verbosity:minimal
if %ERRORLEVEL% EQU 0 (
    echo Build successful!
) else (
    echo Build failed with error code %ERRORLEVEL%
)
pause
