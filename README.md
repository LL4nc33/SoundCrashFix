*** SOUNDCRASHFIX WORKAROUND

This tool patches the "userdata0010" file to fix the sound crash issue when the game is 
not being exited properly by changing the value at offset "0x204E" from "00" to "01".

*** HOW TO USE
Step 1: Place "SoundCrashFix.exe" and "run_soundcrashfix.bat" in the same folder.
Step 2: Edit Batch-File
	1. Open "run_soundcrashfix.bat" in a text editor.
	2. Replace the following:
   		- "C:\path\to\your\shadPS4\user\savedata\1\cusa#####\SPRJ0005\userdata0010" = with full path to "userdata0010" file.
   		- "C:\path\to\your\shadPS4\shadps4.exe" = with full path to "shadps4.exe".
Step 3: Run Tool
	1. Double-Click "run_soundcrashfix.bat".
	2. The Tool will:
		- Patch the "userdata0010" file.
		- Launch "shadps4.exe".

*** NOTES
- Ensure both "SoundCrashFix.exe" and "run_soundcrashfix.bat" are in the same folder.
- If you encounter errors, check that the file paths are correct in the ".bat" file.
- You can do whatever you want with the SourceCode
