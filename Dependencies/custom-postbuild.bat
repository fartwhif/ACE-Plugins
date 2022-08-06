echo on
rem this is a custom post-build script 
rem for after a plugin is built and copied to the ACE dependency folder


rem this is for the secondary server, 
rem primary config files housed in ACE folder, 
rem and secondary config files housed in dependency folder

set src=%~1..\Dependencies\ACE\
set dst=%~1..\Dependencies\ACE2\

set a=%dst%Plugins\ACE.Plugin.Crypto\
set b=%dst%Plugins\ACE.Plugin.Transfer\
set c=%dst%Plugins\ACE.Plugin.Web\

set a2=%~1..\Dependencies\crypto.js
set b2=%~1..\Dependencies\transfer.js
set c2=%~1..\Dependencies\web.js
set d=%~1..\Dependencies\Config.js

IF EXIST %dst% (
	robocopy "%src% " "%dst% " "*.*" /XO /NJH /NP /S

	rem config files have been replaced in secondary folder
	rem restore secondary configuration
	
	IF EXIST %a% (
		IF EXIST %a2% (
			copy /Y %a2% %a%
		)
	)
	IF EXIST %b% (
		IF EXIST %b2% (
			copy /Y %b2% %b%
		)
	)
	IF EXIST %c% (
		IF EXIST %c2% (
			copy /Y %c2% %c%
		)
	)
	IF EXIST %dst% (
		IF EXIST %d% (
			copy /Y %d% %dst%
		)
	)
)

SET ERRORLEVEL = 0