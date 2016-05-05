@echo off

rem Define our properties
set RELEASE_FOLDER=_Release
set DESTINATION_FOLDER=%~dp0..\..\%RELEASE_FOLDER%
set INSTALLER_TEMP=%DESTINATION_FOLDER%\Install Reviewboard
set BIN_FOLDER=%INSTALLER_TEMP%\bin
set EXTERNALS_FOLDER=%~dp0..\..\External
set VERSION_NUMBER=%1
set BUILD_STAMP=%2

rem Create the release folder
rmdir "%DESTINATION_FOLDER%" /S /Q  >nul 2>&1
mkdir "%INSTALLER_TEMP%"

rem Copy over the binaries
mkdir "%BIN_FOLDER%"

rem Process executables
set PROCESS_BUILD_FOLDER=%~dp0..\..\Processes\bin\Release

rem Copy over the applications
xcopy "%PROCESS_BUILD_FOLDER%\Create Review.exe" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\RBProc.exe" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\About.exe" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\Settings.exe" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\Review Stats.exe" "%BIN_FOLDER%" /Q /I >nul 2>&1

rem Shared libraries
xcopy "%PROCESS_BUILD_FOLDER%\RB_Tools.Shared.dll" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\Newtonsoft.Json.dll" "%BIN_FOLDER%" /Q /I >nul 2>&1
xcopy "%PROCESS_BUILD_FOLDER%\CommandLine.dll" "%BIN_FOLDER%" /Q /I >nul 2>&1

rem Copy over the install properties
set INSTALLER_FILES_FOLDER=%~dp0..\..\Installer
xcopy "%INSTALLER_FILES_FOLDER%" "%INSTALLER_TEMP%" /E /Q /I >nul 2>&1

rem Copy over the release notes
set RELEASE_NOTES_FILES_FOLDER=%~dp0..\..\Release Logs
xcopy "%RELEASE_NOTES_FILES_FOLDER%" "%INSTALLER_TEMP%\logs" /E /Q /I >nul 2>&1

rem Update the version number
set VERSION_FOLDER=%INSTALLER_TEMP%\version
mkdir "%VERSION_FOLDER%"

rem Update the build information
echo %VERSION_NUMBER% > "%VERSION_FOLDER%\version"
echo %BUILD_STAMP% >> "%VERSION_FOLDER%\version"


rem Zip up the install package
set ZIP_EXE=%EXTERNALS_FOLDER%\7-zip\7z.exe
"%ZIP_EXE%" a "%DESTINATION_FOLDER%\Install Reviewboard %VERSION_NUMBER%.zip" "%INSTALLER_TEMP%" -r -mx9

rem Lose the installer temp folder
rmdir "%INSTALLER_TEMP%" /S /Q