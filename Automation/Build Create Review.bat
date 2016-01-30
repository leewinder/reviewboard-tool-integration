@echo off

rem Pull in the properties
set REVIEWBOARD_SERVER=%1
set JIRA_SERVER=%2

rem We need to have both or we ask for both
if not [%REVIEWBOARD_SERVER%] == [] if not [%JIRA_SERVER%] == [] goto server_address_aquired

rem Request our servers
set /p REVIEWBOARD_SERVER="Set Reviewboard Server (Blank for default): "
set /p JIRA_SERVER="Set Jira Server (Blank for default): "

:server_address_aquired

rem Get the expected server
set DEFAULT_RB_SERVER=http://localhost/reviewboard
set DEFAULT_JIRA_SERVER=http://localhost/jira

rem Set the sever we need to use
if [%REVIEWBOARD_SERVER%] == [] set REVIEWBOARD_SERVER=%DEFAULT_RB_SERVER%
if [%JIRA_SERVER%] == [] set JIRA_SERVER=%DEFAULT_JIRA_SERVER%

rem Update the files with the server we are interested in using
set FART_EXE=%~dp0..\External\Fart\fart.exe

rem Update the reviewboard server
set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\Settings\ReviewAuth.Designer.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_RB_SERVER%" "%REVIEWBOARD_SERVER%"

set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\Settings\ReviewAuth.settings
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_RB_SERVER%" "%REVIEWBOARD_SERVER%"

set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\app.config
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_RB_SERVER%" "%REVIEWBOARD_SERVER%"

rem Update the Jira server
set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\Settings\Jira.Designer.cs
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_JIRA_SERVER%" "%JIRA_SERVER%"

set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\Settings\Jira.settings
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_JIRA_SERVER%" "%JIRA_SERVER%"

set THIS_FILE=%~dp0..\Processes\Create Review\Create Review\app.config
"%FART_EXE%" -i -q "%THIS_FILE%" "%DEFAULT_JIRA_SERVER%" "%JIRA_SERVER%"

rem Build the executable
"%~dp0.\Templates\Build Process.bat" "Create Review" Release