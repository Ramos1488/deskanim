@echo off
setlocal
rem Requirements: .NET 8 SDK and Inno Setup 6 (https://jrsoftware.org/isdl.php)

dotnet publish DeskAnim.csproj -c Release -r win-x64 --self-contained true -o publish
if errorlevel 1 goto :fail

set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" set "ISCC=%ProgramFiles%\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" set "ISCC=%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" (
  echo Inno Setup 6 not found. Install it from https://jrsoftware.org/isdl.php
  goto :fail
)

"%ISCC%" installer\DeskAnim.iss
if errorlevel 1 goto :fail

echo.
echo Done: dist\DeskAnim-Setup-0.1.0.exe
if "%CI%"=="" pause
exit /b 0

:fail
echo Build failed.
if "%CI%"=="" pause
exit /b 1
