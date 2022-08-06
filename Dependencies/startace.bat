@echo off
echo %~dp0
set dp1=%~dp0ACE\
set dp2=%~dp0ACE2\
set srv1=%~dp0ACE\ACE.Server.exe
set srv2=%~dp0ACE2\ACE.Server.exe

IF EXIST %srv1% (
	echo %srv1%
	cd %dp1%
	start "server1" "%srv1%"
)

IF EXIST %srv2% (
	echo %srv2%
	cd %dp2%
	start "server2" "%srv2%"
)

rem pause