@echo off

del /q gen\*

setlocal enabledelayedexpansion

for %%i in (bin\*.proto) do (
	
	set old_name=%%i
	set new_name=!old_name:proto=pb!
	set new_name=!new_name:bin=gen!

    protoc25 --descriptor_set_out !new_name! !old_name!

)


echo "finished"
pause