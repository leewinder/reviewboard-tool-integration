@echo off

rem Pull in the properties
set VERSION_NUMBER=%1
set REVIEWBOARD_SERVER=%2

rem We need to have both or we ask for both
if not [%REVIEWBOARD_SERVER%] == [] if not [%JIRA_SERVER%] == [] goto properties_aquired

rem Get our build information
set /p VERSION_NUMBER="Specify Version Number: "
set /p REVIEWBOARD_SERVER="Set Reviewboard Server (Blank for default): "

:properties_aquired

rem Get the expected server
set DEFAULT_RB_SERVER=http://localhost/reviewboard
if [%REVIEWBOARD_SERVER%] == [] set REVIEWBOARD_SERVER=%DEFAULT_RB_SERVER%

rem Update the files with the server we are interested in using
set FART_EXE=%~dp0..\External\Fart\fart.exe

rem Update the reviewboard server
set THIS_FILE=%~dp0..\Installer\bin\open_browser.bat
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_RB_SERVER%" "%REVIEWBOARD_SERVER%"

set THIS_FILE=%~dp0..\Installer\install.bat
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_RB_SERVER%" "%REVIEWBOARD_SERVER%"

set THIS_FILE=%~dp0..\Release Logs\release_notes.html
"%FART_EXE%" -i -q "%THIS_FILE%" "__VERSION__NUMBER__" "%VERSION_NUMBER%"

rem Build the installer
"%~dp0.\Templates\Package Installer.bat" %VERSION_NUMBER%