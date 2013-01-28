@echo off
set version=%1

if "%version%"=="" goto syntax

REM requires 7za, git and msbuild to be in path
REM make sure we can find sqlcmd first...
7za -h > nul
IF ERRORLEVEL 1 echo could not find 7za on this machine.  exiting. & goto Done

git --version > nul
IF ERRORLEVEL 1 echo could not find git on this machine.  exiting. & goto Done

msbuild /? > nul
IF ERRORLEVEL 1 echo could not find msbuild on this machine.  exiting. & goto Done

msbuild ScriptDb.csproj /property:Configuration=Release
IF ERRORLEVEL 1 echo Build failed & goto Done

set releasenamebase=ScriptDB-%version%
set targetdir=releases\%releasenamebase%

mkdir %targetdir%
copy bin\Release\ScriptDB.exe %targetdir%
copy builddb*.bat %targetdir%

set zipname=%releasenamebase%.zip
set zippath=releases\%zipname%

IF EXIST %zippath% del %zippath%
7za a -tzip -r %zippath% -w releases %releasenamebase%

IF ERRORLEVEL 1 echo Failed to Zip & goto Done
git tag -f v%version%

goto Done

:Syntax
echo.
echo buildrelease.bat syntax:
echo.
echo buildrelease.bat version
echo.
echo where:
echo.
echo    version:  is the desired version tag
goto Done

:Done
