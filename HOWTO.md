# How to Use SoundCrashFix for Bloodborne

A step-by-step guide to fix sound crashes in Bloodborne on shadPS4 emulator.

## Prerequisites

Before starting, make sure you have:
- ✅ shadPS4 emulator installed
- ✅ Bloodborne configured and running in shadPS4
- ✅ Played Bloodborne at least once (to create save files)
- ✅ Windows OS with .NET Framework 4.7.2+

## Quick Start Guide

### Step 1: Download the Tool

1. Download `SoundCrashFix.exe` and `run_soundcrashfix.bat`
2. Place both files in the same folder (e.g., `C:\Tools\SoundCrashFix\`)

### Step 2: Locate Your Files

You need to find two important paths:

#### A. Find your Bloodborne save file:
```
[shadPS4 folder]\user\savedata\1\[game version]\SPRJ0005\userdata0010
```

Example paths:
- `C:\shadPS4\user\savedata\1\CUSA00207\SPRJ0005\userdata0010`
- `C:\Emulators\shadPS4\user\savedata\1\CUSA00900\SPRJ0005\userdata0010`

#### B. Find your shadPS4 executable:
```
[shadPS4 folder]\shadps4.exe
```

Example:
- `C:\shadPS4\shadps4.exe`
- `C:\Emulators\shadPS4\shadps4.exe`

### Step 3: Configure the Batch File

1. Right-click `run_soundcrashfix.bat` and select **Edit** (or open in Notepad)

2. Find these lines:
```batch
set FILEPATH="C:\path\to\your\shadPS4\user\savedata\1\cusa#####\SPRJ0005\userdata0010"
set SHADPS4_DIR="C:\path\to\your\shadPS4"
set SHADPS4_EXE="C:\path\to\your\shadPS4\shadps4.exe"
```

3. Replace with your actual paths. For example:
```batch
set FILEPATH="C:\shadPS4\user\savedata\1\CUSA00207\SPRJ0005\userdata0010"
set SHADPS4_DIR="C:\shadPS4"
set SHADPS4_EXE="C:\shadPS4\shadps4.exe"
```

4. **Save** the file and close the editor

### Step 4: Run the Fix

1. **Double-click** `run_soundcrashfix.bat`
2. You'll see a console window with status messages
3. The tool will:
   - ✅ Patch your save file
   - ✅ Launch shadPS4 automatically
   - ✅ Show "Done!" when complete

4. Press **ESC** to close the patch tool window
5. Bloodborne will launch in shadPS4 without sound crashes!

## Manual Usage (Advanced)

If you prefer to run the tool manually:

```cmd
SoundCrashFix.exe "C:\path\to\userdata0010"
```

Then launch shadPS4 separately.

## Verification

To verify the patch worked:
- The tool will display: `Value at offset 0x204E changed from 00 to 01`
- Or if already patched: `No changes needed. The value at offset 0x204E is already 01`

## Important Notes

⚠️ **Backup Your Saves**: Always backup your `userdata0010` file before patching

⚠️ **One-Time Fix**: You only need to run this once per save file

⚠️ **Game Version**: The `cusa#####` folder name depends on your Bloodborne version:
- CUSA00207 (EU version)
- CUSA00900 (US version)
- Other region codes may vary

## Folder Structure Example

```
C:\shadPS4\                          # Your shadPS4 folder
├── shadps4.exe                      # Emulator executable
└── user\
    └── savedata\
        └── 1\
            └── CUSA00207\           # Game version folder
                └── SPRJ0005\        # Bloodborne save folder
                    └── userdata0010 # File to patch
```

## Troubleshooting

### Problem: "File not found" error

**Solution**: 
- Check your file paths in the .bat file
- Make sure you've played Bloodborne at least once
- Verify the `SPRJ0005` folder exists

### Problem: "Access denied" error

**Solution**:
- Close shadPS4 if it's running
- Run as Administrator (right-click → Run as administrator)

### Problem: Sound still crashes

**Solution**:
- Verify the patch was applied (run tool again to check)
- Make sure you're patching the correct save file
- Try deleting the save and creating a new one, then patch again

### Problem: Can't find save files

**Solution**:
1. Launch Bloodborne in shadPS4
2. Create a new game or load existing
3. Save and exit the game
4. Check the savedata folder again

## Tips

- 💡 Create a desktop shortcut to `run_soundcrashfix.bat` for easy access
- 💡 The patch persists - you don't need to run it every time
- 💡 If you create a new save file, you'll need to patch it too
- 💡 Keep the tool handy in case shadPS4 updates reset your saves

## Support

- **Nexus Mods**: https://www.nexusmods.com/bloodborne/mods/165
- **GitHub**: https://github.com/LL4nc33/SoundCrashFix

---

*Happy hunting, and enjoy Bloodborne without sound crashes!* 🎮