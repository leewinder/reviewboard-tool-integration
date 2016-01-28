@echo off

:::::::::::::::::::::::::::::::::::::::::
:: Start point
:::::::::::::::::::::::::::::::::::::::::

CLS
echo.
echo. Starting Reviewboard Integration Tools installer
echo.

:::::::::::::::::::::::::::::::::::::::::
:: Automatically check & get admin rights
:::::::::::::::::::::::::::::::::::::::::
:checkPrivileges
NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )

:getPrivileges
if '%1'=='ELEV' (echo ELEV & shift /1 & goto gotPrivileges)

setlocal DisableDelayedExpansion
set "batchPath=%~0"
setlocal EnableDelayedExpansion
ECHO Set UAC = CreateObject^("Shell.Application"^) > "%temp%\OEgetPrivileges.vbs"
ECHO args = "ELEV " >> "%temp%\OEgetPrivileges.vbs"
ECHO For Each strArg in WScript.Arguments >> "%temp%\OEgetPrivileges.vbs"
ECHO args = args ^& strArg ^& " "  >> "%temp%\OEgetPrivileges.vbs"
ECHO Next >> "%temp%\OEgetPrivileges.vbs"
ECHO UAC.ShellExecute "!batchPath!", args, "", "runas", 1 >> "%temp%\OEgetPrivileges.vbs"
"%SystemRoot%\System32\WScript.exe" "%temp%\OEgetPrivileges.vbs" %*
exit /B

:gotPrivileges
if '%1'=='ELEV' shift /1
setlocal & pushd .
cd /d %~dp0


::::::::::::::::::::::::::::
:: Initial properties
::::::::::::::::::::::::::::

rem Local properties
setlocal EnableDelayedExpansion

rem Global properties
set RESTART_REQUIRED=0
set SHOW_RELEASE_NOTES=0
set SHOW_RB_SERVER=0



::::::::::::::::::::::::::::
:: .net install
::::::::::::::::::::::::::::

reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs\.NETFramework,Version=v4.5" >nul 2>&1
if %ERRORLEVEL% EQU 0 goto skip_dotnet_4point5

:install_dotnet_4point5

rem Install the .net tools
echo msgbox "Microsoft .Net Framework 4.5 is required for ReviewBoard.  The Web Installer will now open and install .net if required" > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

"%~dp0\installs\dotNetFx45_Full_setup.exe"

:skip_dotnet_4point5


::::::::::::::::::::::::::::
:: Diff tools install
::::::::::::::::::::::::::::

rem Checking if the diff tools are installed
diff -v >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
	rem Because we're installing the diff tools, we'll need to restart
	set RESTART_REQUIRED=1
	
    goto install_diff_tools
)

rem diff is installed, but is it the right version?
set search_string=diff (GNU diffutils) 2.8.7

diff -v | find /i "%search_string%" > nul
if %ERRORLEVEL% EQU 0 goto skip_diff_tools_install

:install_diff_tools



rem Install the diff tools
echo msgbox "An update to the GnuWin32 Diff tools will now be installed.  Please install with the default settings and don't change anything!" > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

"%~dp0\installs\diffutils-2.8.7-1.exe"

rem Pull out the environment variable from the registry so we can avoid the 1024 limit of setx
set ENVIRONMENT_REG_PATH=HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment
set ENVIRONMENT_REG_KEY=Path

FOR /F "usebackq tokens=2,* skip=2" %%L IN (
    `reg query "%ENVIRONMENT_REG_PATH%" /v %ENVIRONMENT_REG_KEY%`
) DO SET CURRENT_PATH_VALUES=%%M

rem If the path is not already in the list, add it
set ADDITIONAL_PATH=C:\Program Files (x86)\GnuWin32\bin
If NOT "%CURRENT_PATH_VALUES%"=="!CURRENT_PATH_VALUES:%ADDITIONAL_PATH%=!" (
    goto skip_diff_tools_install
)

rem Add the path to the environment
reg add "%ENVIRONMENT_REG_PATH%" /v %ENVIRONMENT_REG_KEY% /t REG_SZ /d "%CURRENT_PATH_VALUES%;%ADDITIONAL_PATH%" /f  >nul 2>&1

:skip_diff_tools_install



::::::::::::::::::::::::::::
:: SVN install
::::::::::::::::::::::::::::

rem Checking if svn is installed, if not install Tortoise
svn --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 goto install_tortoise_svn

rem SVN is installed, but is it the right version?
set search_string=1.8.14
svn --version | find /i "%search_string%" > nul
if %ERRORLEVEL% EQU 0 goto skip_tortoise_svn_install

:install_tortoise_svn

rem Install Tortoise SVN
echo msgbox "An update to TortoiseSVN will now be installed.  When going through the install process, please make sure the 'Command Line Client Tools' option is ticked during install" > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

"%~dp0\installs\TortoiseSVN-1.8.12.26645-x64-svn-1.8.14.msi"

:skip_tortoise_svn_install



::::::::::::::::::::::::::::
:: Reviewboard tools
::::::::::::::::::::::::::::

rem Checking if the diff tools are installed
call rbt -v >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
	rem We need to install Reviewboard regardless
	rem Since we don't see the RB tools, assume we've installed and signed up previously
	set SHOW_RB_SERVER=1
    goto install_reviewboard_tools
) else (
    rem Reviewboard is already installed
	rem This means we've run this before so just show release notes
	set SHOW_RELEASE_NOTES=1
) 

rem RBTools is installed but what version?
set search_string=RBTools 0.7.5
call rbt -v 2>&1 | find /i "%search_string%" > nul
If %ERRORLEVEL% EQU 0 (
    rem The right version is installed
	goto skip_reviewboard_tools_install
) else (
    rem The wrong version is installed
)

:install_reviewboard_tools

rem Install RBTools
echo msgbox "An update to RBTools is required.  Please install it with the default options." > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

"%~dp0\installs\RBTools-0.7.5.exe"

:skip_reviewboard_tools_install



::::::::::::::::::::::::::::
:: Local file copies
::::::::::::::::::::::::::::

rem Get the review board folder
rmdir "C:\Program Files\Reviewboard\" /s /q

mkdir "C:\Program Files\Reviewboard" >nul 2>&1
mkdir "C:\Program Files\Reviewboard\bin" >nul 2>&1
mkdir "C:\Program Files\Reviewboard\data" >nul 2>&1

rem copy the assets over for the folder hooks
xcopy "%~dp0\data\*" "C:\Program Files\Reviewboard\data" >nul 2>&1
xcopy "%~dp0\bin\*" "C:\Program Files\Reviewboard\bin" >nul 2>&1



::::::::::::::::::::::::::::
::  Version information
::::::::::::::::::::::::::::
xcopy "%~dp0\version\*" "C:\Program Files\Reviewboard" >nul 2>&1



::::::::::::::::::::::::::::
::  Registry installs
::::::::::::::::::::::::::::

rem Install the registry entries
regedit /s "%~dp0\registry\open_review_board.reg"
regedit /s "%~dp0\registry\create_review.reg"
regedit /s "%~dp0\registry\open_about_box.reg"

regedit /s "%~dp0\registry\review_board_menu.reg"



::::::::::::::::::::::::::::
:: Release notes
::::::::::::::::::::::::::::

rem Show release notes
If %SHOW_RELEASE_NOTES%==0 (
	goto skip_show_release_notes
)

set RELEASE_NOTES_PATH=file:///%~dp0logs\release_notes.html
set SPACE_REPLACEMENT=%%20
set RELEASE_NOTES_PATH=%RELEASE_NOTES_PATH: =!SPACE_REPLACEMENT!%
start %RELEASE_NOTES_PATH%

:skip_show_release_notes

::::::::::::::::::::::::::::
:: Opening Reviewboard server
::::::::::::::::::::::::::::

rem Open up the review board browser to sign up
If %SHOW_RB_SERVER%==0 (
	goto skip_show_rb_server
)

echo msgbox "The Reviewboard server will now open up in your browser.  Please sign up if you have not already done so." > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

start http://localhost/reviewboard

:skip_show_rb_server


::::::::::::::::::::::::::::
:: User to restart message
::::::::::::::::::::::::::::

rem Tell the users to restart if we need to
If %RESTART_REQUIRED%==0 (
	goto skip_show_restart
)

echo msgbox "Windows now requires that you restart your machine" > %tmp%\tmp.vbs
cscript /nologo %tmp%\tmp.vbs
del %tmp%\tmp.vbs

:skip_show_restart


::::::::::::::::::::::::::::
:: Wait for a while
::::::::::::::::::::::::::::
echo.
echo. Reviewboard Integration Tools successfully installed
echo. Press any key to continue
echo.
timeout /t 10 >nul