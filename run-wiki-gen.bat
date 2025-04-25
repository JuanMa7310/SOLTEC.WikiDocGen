@echo off
setlocal
cd /d %~dp0

echo 📦 Building SOLTEC.WikiDocGen...
dotnet build SOLTEC.WikiDocGen.csproj -c Release
if %errorlevel% neq 0 (
    echo ❌ Build failed. Aborting.
    exit /b 1
)

echo 🚀 Running Wiki Generator...
dotnet run --project SOLTEC.WikiDocGen.csproj --configuration Release

endlocal
pause