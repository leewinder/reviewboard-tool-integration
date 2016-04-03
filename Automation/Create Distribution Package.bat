@echo off

rem Pull in the properties
set VERSION_NUMBER=%1
set REVIEWBOARD_SERVER=%2
set JIRA_SERVER=%3

rem We need to have both or we ask for both
if not [%VERSION_NUMBER%] == [] if not [%REVIEWBOARD_SERVER%] == [] if not [%JIRA_SERVER%] == [] goto properties_aquired

rem Get our build information
set /p VERSION_NUMBER="Specify Version Number (e.g. 1.2.0): "
set /p REVIEWBOARD_SERVER="Set Reviewboard Server (leave blank for default): "
set /p JIRA_SERVER="Set Jira Server (leave blank for default): "

:properties_aquired

rem Get the expected servers
set DEFAULT_REVIEWBOARD_SERVER=http://localhost/reviewboard
if [%REVIEWBOARD_SERVER%] == [] set REVIEWBOARD_SERVER=%DEFAULT_REVIEWBOARD_SERVER%

set DEFAULT_JIRA_SERVER=http://localhost/jira
if [%JIRA_SERVER%] == [] set JIRA_SERVER=%DEFAULT_JIRA_SERVER%

set DEFAULT_VERSION_NUMBER=9.9.9.9

rem Update the files with the server we are interested in using
set FART_EXE=%~dp0..\External\Fart\fart.exe

rem Update the open RB server option
set THIS_FILE=%~dp0..\Installer\bin\open_browser.bat
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_REVIEWBOARD_SERVER%" "%REVIEWBOARD_SERVER%"

rem Update the installer
set THIS_FILE=%~dp0..\Installer\install.bat
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_REVIEWBOARD_SERVER%" "%REVIEWBOARD_SERVER%"

rem Update the server addresses in the processes
set THIS_FILE=%~dp0..\Processes\Shared\Shared\Server\Names.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_REVIEWBOARD_SERVER%" "%REVIEWBOARD_SERVER%"
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_JIRA_SERVER%" "%JIRA_SERVER%"

rem Update the process assembly versions
set THIS_FILE=%~dp0..\Processes\About\About\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

set THIS_FILE=%~dp0..\Processes\Authentication\Authentication\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

set THIS_FILE=%~dp0..\Processes\RBProc\RBProc\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

set THIS_FILE=%~dp0..\Processes\Review Stats\Review Stats\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

set THIS_FILE=%~dp0..\Processes\Shared\Shared\Properties\AssemblyInfo.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_VERSION_NUMBER%" "%VERSION_NUMBER%.0"

rem Update the release notes
set THIS_FILE=%~dp0..\Release Logs\release_notes.html
"%FART_EXE%" -i -q "%THIS_FILE%" "__VERSION__NUMBER__" "%VERSION_NUMBER%"

rem Build the processes
call "%~dp0.\Build About.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build Authentication.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build Create Review.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build Review Stats.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build RBProc.bat"
if %errorlevel% neq 0 goto :error_building_process

rem Build the installer
"%~dp0.\Templates\Package Installer.bat" %VERSION_NUMBER%

rem Done
exit /b 0

: error_building_process
echo.
echo Unable to build all processes - see output for more information
echo.
pause