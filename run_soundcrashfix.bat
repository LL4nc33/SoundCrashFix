@echo off
rem ===================================================
rem Batch file to run SoundCrashFix.exe and shadps4.exe
rem ===================================================

rem Specify the path to your userdata0010 file (no file extension)
set FILEPATH="C:\path\to\your\shadPS4\user\savedata\1\cusa#####\SPRJ0005\userdata0010"

rem Specify directory where shadps4.exe is located
set SHADPS4_DIR="C:\path\to\your\shadPS4"

rem Specify the path to shadps4.exe
set SHADPS4_EXE="C:\path\to\your\shadPS4\shadps4.exe"

rem Check if the userdata0010 file exists
if not exist %FILEPATH% (
    echo Error: The file %FILEPATH% does not exist. Please check the path.
    pause
    exit /b
)

rem Check if shadps4.exe exists
if not exist %SHADPS4_EXE% (
    echo Error: The file %SHADPS4_EXE% does not exist. Please check the path.
    pause
    exit /b
)

rem Run SoundCrashFix.exe with the specified userdata0010 path
SoundCrashFix.exe %FILEPATH%

rem Run shadps4.exe with the correct working directory
cd /d %SHADPS4_DIR%
start "" shadps4.exe

rem Pause to let the user see the program output
pause
