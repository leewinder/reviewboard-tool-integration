@echo off

rem Pull in the properties
set REVIEWBOARD_SERVER=%1
set JIRA_SERVER=%2

rem We need to have both or we ask for both
if not [%REVIEWBOARD_SERVER%] == [] if not [%JIRA_SERVER%] == [] goto properties_aquired

rem Get our build information
set /p REVIEWBOARD_SERVER="Set Reviewboard Server (leave blank for default): "
set /p JIRA_SERVER="Set Jira Server (leave blank for default): "

:properties_aquired

rem Get our build flag
set BUILD_FLAG=%3
if [%BUILD_FLAG%] == [] set BUILD_FLAG=unknown

rem Get the expected servers
set DEFAULT_REVIEWBOARD_SERVER=http://localhost/reviewboard
if [%REVIEWBOARD_SERVER%] == [] set REVIEWBOARD_SERVER=%DEFAULT_REVIEWBOARD_SERVER%

set DEFAULT_JIRA_SERVER=http://localhost/jira
if [%JIRA_SERVER%] == [] set JIRA_SERVER=%DEFAULT_JIRA_SERVER%

rem Get the version number
set /p VERSION_NUMBER=< %~dp0..\version.txt
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

set THIS_FILE=%~dp0..\Processes\Settings\Settings\Properties\AssemblyInfo.cs
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

call "%~dp0.\Build Settings.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build Create Review.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build Review Stats.bat"
if %errorlevel% neq 0 goto :error_building_process

call "%~dp0.\Build RBProc.bat"
if %errorlevel% neq 0 goto :error_building_process

rem Pull out the date and time for the build number
SET HOUR=%time:~0,2%
SET DT_STAMP_9=%date:~-4%%date:~3,2%%date:~0,2%0%time:~1,1%%time:~3,2%%time:~6,2% 
SET DT_STAMP_24=%date:~-4%%date:~3,2%%date:~0,2%%time:~0,2%%time:~3,2%%time:~6,2%
if "%HOUR:~0,1%" == " " (SET DT_FINAL_TIME_STAMP=%DT_STAMP_9%) else (SET DT_FINAL_TIME_STAMP=%DT_STAMP_24%)
set DT_FINAL_TIME_STAMP=%DT_FINAL_TIME_STAMP:~2%

rem Get the Git sha of the current commit
FOR /F "delims=" %%i IN (
    'git -C "%~dp0 " log --pretty^=format:%%h -n 1'
) DO (
    set GIT_SHA=%%i
)

rem Create our build stamp
set BUILD_STAMP=%DT_FINAL_TIME_STAMP%.%GIT_SHA%.%BUILD_FLAG%

rem Build the installer
"%~dp0.\Templates\Package Installer.bat" %VERSION_NUMBER% %BUILD_STAMP%

rem Done
exit /b 0

: error_building_process
echo.
echo Unable to build all processes - see output for more information
echo.
pause