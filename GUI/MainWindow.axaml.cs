using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SoundCrashFixGUI
{
    public partial class MainWindow : Window
    {
        private const int SOUND_CRASH_OFFSET = 0x204E;
        private const byte PATCHED_VALUE = 0x01;
        private const string SETTINGS_FILE = "gui_settings.json";
        private const string EXPECTED_FILENAME = "userdata0010";
        
        private string? _saveFilePath;
        private string? _emulatorPath;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeEventHandlers();
            LoadSavedSettings();
            DetectPaths();
            
            // Ensure initial validation after loading settings and detection
            ValidateSaveFile();
        }

        private void InitializeEventHandlers()
        {
            BrowseSaveButton!.Click += BrowseSaveButton_Click;
            BrowseEmulatorButton!.Click += BrowseEmulatorButton_Click;
            SaveSettingsButton!.Click += SaveSettingsButton_Click;
            PatchButton!.Click += PatchButton_Click;
            
            // Window controls
            MinimizeButton!.Click += (s, e) => WindowState = WindowState.Minimized;
            CloseButton!.Click += (s, e) => Close();
            
            // Title bar drag
            TitleBarDragArea!.PointerPressed += (s, e) => BeginMoveDrag(e);
        }

        private void LoadSavedSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    var settings = System.Text.Json.JsonSerializer.Deserialize<GuiSettings>(json);
                    if (settings != null)
                    {
                        if (!string.IsNullOrEmpty(settings.SavePath) && File.Exists(settings.SavePath))
                        {
                            _saveFilePath = settings.SavePath;
                            SavePathTextBox!.Text = _saveFilePath;
                            ValidateSaveFile();
                        }
                        if (!string.IsNullOrEmpty(settings.EmulatorPath) && File.Exists(settings.EmulatorPath))
                        {
                            _emulatorPath = settings.EmulatorPath;
                            EmulatorPathTextBox!.Text = _emulatorPath;
                        }
                        AutoLaunchCheckBox!.IsChecked = settings.AutoLaunch;
                        AutoSaveSettingsCheckBox!.IsChecked = settings.AutoSave;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load settings", ex);
            }
        }

        private void DetectPaths()
        {
            string[] commonPaths = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                ? new[] { @"C:\shadPS4", @"D:\shadPS4" }
                : new[] { "/home/deck/shadPS4", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "shadPS4") };

            foreach (var path in commonPaths)
            {
                if (Directory.Exists(path))
                {
                    var saveBasePath = Path.Combine(path, "user", "savedata", "1");
                    if (Directory.Exists(saveBasePath))
                    {
                        var saves = Directory.GetFiles(saveBasePath, EXPECTED_FILENAME, SearchOption.AllDirectories);
                        if (saves.Length > 0 && _saveFilePath == null)
                        {
                            _saveFilePath = saves[0];
                            SavePathTextBox!.Text = _saveFilePath;
                            ValidateSaveFile();
                        }
                    }

                    var emulatorName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "shadps4.exe" : "shadps4";
                    var emulatorPath = Path.Combine(path, emulatorName);
                    if (File.Exists(emulatorPath) && _emulatorPath == null)
                    {
                        _emulatorPath = emulatorPath;
                        EmulatorPathTextBox!.Text = _emulatorPath;
                    }
                }
            }
        }

        private async void BrowseSaveButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.StorageProvider == null)
                {
                    ShowError("File picker unavailable", new InvalidOperationException("Storage provider not available"));
                    return;
                }

                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select userdata0010 file",
                    AllowMultiple = false
                });

                if (files.Count > 0 && files[0]?.Path?.LocalPath != null)
                {
                    _saveFilePath = files[0].Path.LocalPath;
                    if (SavePathTextBox != null)
                        SavePathTextBox.Text = _saveFilePath;
                    ValidateSaveFile();
                    AutoSaveIfEnabled();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to select save file", ex);
            }
        }

        private async void BrowseEmulatorButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.StorageProvider == null)
                {
                    ShowError("File picker unavailable", new InvalidOperationException("Storage provider not available"));
                    return;
                }

                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select shadPS4 executable",
                    AllowMultiple = false
                });

                if (files.Count > 0 && files[0]?.Path?.LocalPath != null)
                {
                    _emulatorPath = files[0].Path.LocalPath;
                    if (EmulatorPathTextBox != null)
                        EmulatorPathTextBox.Text = _emulatorPath;
                    ValidateSaveFile();
                    AutoSaveIfEnabled();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to select emulator", ex);
            }
        }

        private bool IsValidSaveFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
                    
                var fileName = Path.GetFileName(filePath);
                if (!string.Equals(fileName, EXPECTED_FILENAME, StringComparison.OrdinalIgnoreCase))
                    return false;
                    
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < SOUND_CRASH_OFFSET + 1)
                    return false;
                    
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void ValidateSaveFile()
        {
            if (!string.IsNullOrEmpty(_saveFilePath) && File.Exists(_saveFilePath) && 
                SaveStatusText != null && StatusTitle != null && StatusMessage != null && 
                PatchButton != null && TechnicalDetails != null)
            {
                if (!IsValidSaveFile(_saveFilePath))
                {
                    SaveStatusText!.Text = "Invalid save file";
                    StatusTitle!.Text = "Status: Error";
                    StatusMessage!.Text = "Selected file is not a valid userdata0010 save file";
                    PatchButton!.IsEnabled = false;
                    PatchButton!.Content = "PATCH";
                    TechnicalDetails!.IsVisible = false;
                    return;
                }
                
                try
                {
                    using var fs = new FileStream(_saveFilePath, FileMode.Open, FileAccess.Read);
                    fs.Seek(SOUND_CRASH_OFFSET, SeekOrigin.Begin);
                    byte currentValue = (byte)fs.ReadByte();
                    
                    TechnicalDetails!.IsVisible = false;
                    
                    if (currentValue == PATCHED_VALUE)
                    {
                        SaveStatusText.Text = "Already patched";
                        StatusTitle!.Text = "Status: Ready";
                        StatusTitle.Foreground = new SolidColorBrush(Color.Parse("#00FF00")); // Green
                        StatusMessage!.Text = $"(current value: 0x{currentValue:X2})";
                        
                        // Button logic for already patched files
                        if (!string.IsNullOrEmpty(_emulatorPath) && File.Exists(_emulatorPath))
                        {
                            PatchButton!.Content = "START";
                        }
                        else
                        {
                            PatchButton!.Content = "RE-PATCH";
                        }
                    }
                    else
                    {
                        SaveStatusText.Text = "Ready to patch";
                        StatusTitle!.Text = "Status: Needs patching";
                        StatusTitle.Foreground = new SolidColorBrush(Color.Parse("#FFA500")); // Orange
                        StatusMessage!.Text = $"(current value: 0x{currentValue:X2}, target: 0x01)";
                        PatchButton!.Content = "PATCH";
                    }
                    
                    PatchButton!.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    SaveStatusText!.Text = "Error reading file";
                    StatusTitle!.Text = "Status: Error";
                    StatusMessage!.Text = $"Unable to read save file: {ex.Message}";
                    PatchButton!.IsEnabled = false;
                    PatchButton!.Content = "PATCH";
                    TechnicalDetails!.IsVisible = false;
                }
            }
            else
            {
                if (SaveStatusText != null)
                    SaveStatusText.Text = "No file selected";
                if (StatusTitle != null)
                    StatusTitle.Text = "Status: Waiting for save file";
                if (StatusMessage != null)
                    StatusMessage.Text = "Select a save file to begin";
                if (PatchButton != null)
                {
                    PatchButton.IsEnabled = false;
                    PatchButton.Content = "PATCH";
                }
                if (TechnicalDetails != null)
                    TechnicalDetails.IsVisible = false;
            }
        }

        private async void PatchButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_saveFilePath)) return;

            var buttonText = PatchButton!.Content?.ToString();
            
            // If button says START, just launch emulator
            if (buttonText == "START")
            {
                if (!string.IsNullOrEmpty(_emulatorPath) && File.Exists(_emulatorPath))
                {
                    PatchButton!.IsEnabled = false;
                    PatchButton.Content = "Starting...";
                    await Task.Delay(300);
                    
                    try
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = _emulatorPath,
                            WorkingDirectory = Path.GetDirectoryName(_emulatorPath) ?? "",
                            UseShellExecute = true
                        };
                        Process.Start(startInfo);
                        
                        StatusTitle!.Text = "Status: Launched";
                        StatusMessage!.Text = "Emulator started successfully.";
                        PatchButton.Content = "START";
                        PatchButton!.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        StatusTitle!.Text = "Status: Error";
                        StatusMessage!.Text = $"Failed to start emulator: {ex.Message}";
                        PatchButton.Content = "START";
                        PatchButton!.IsEnabled = true;
                    }
                }
                return;
            }

            // Regular patching logic
            PatchButton!.IsEnabled = false;
            PatchButton.Content = "Processing...";
            await Task.Delay(300);

            try
            {
                using (var fs = new FileStream(_saveFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    // Create backup before patching
                    var backupPath = _saveFilePath + ".backup";
                    if (!File.Exists(backupPath))
                    {
                        File.Copy(_saveFilePath, backupPath, false);
                    }
                    
                    fs.Seek(SOUND_CRASH_OFFSET, SeekOrigin.Begin);
                    byte currentValue = (byte)fs.ReadByte();
                    
                    fs.Seek(SOUND_CRASH_OFFSET, SeekOrigin.Begin);
                    fs.WriteByte(PATCHED_VALUE);
                    
                    StatusTitle!.Text = "Status: Success";
                    StatusMessage!.Text = $"Successfully patched! Backup created at: {Path.GetFileName(backupPath)}";
                    
                    PatchButton.Content = "Success";
                    await Task.Delay(1000);
                }

                if (AutoLaunchCheckBox!.IsChecked == true && !string.IsNullOrEmpty(_emulatorPath))
                {
                    try
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = _emulatorPath,
                            WorkingDirectory = Path.GetDirectoryName(_emulatorPath) ?? "",
                            UseShellExecute = true
                        };
                        Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        ShowError("Failed to start emulator", ex);
                    }
                }
                
                ValidateSaveFile();
            }
            catch (Exception ex)
            {
                PatchButton!.IsEnabled = true;
                PatchButton.Content = "PATCH";
                StatusTitle!.Text = "Status: Failed";
                StatusMessage!.Text = $"Failed to patch: {ex.Message}";
                ShowError("Patch operation failed", ex);
            }
        }

        private async void SaveSettingsButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                await SaveSettingsAsync();
                
                var originalText = SaveSettingsButton!.Content;
                SaveSettingsButton.Content = "Saved";
                await Task.Delay(1000);
                SaveSettingsButton.Content = originalText;
            }
            catch (Exception ex)
            {
                ShowError("Failed to save settings", ex);
            }
        }

        private async void AutoSaveIfEnabled()
        {
            if (AutoSaveSettingsCheckBox?.IsChecked == true)
            {
                try
                {
                    await SaveSettingsAsync();
                }
                catch (Exception ex)
                {
                    ShowError("Auto-save failed", ex);
                }
            }
        }
        
        private async Task SaveSettingsAsync()
        {
            var settings = new GuiSettings
            {
                SavePath = _saveFilePath ?? "",
                EmulatorPath = _emulatorPath ?? "",
                AutoLaunch = AutoLaunchCheckBox?.IsChecked ?? true,
                AutoSave = AutoSaveSettingsCheckBox?.IsChecked ?? true
            };
            
            var json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(SETTINGS_FILE, json);
        }
        
        private void ShowError(string message, Exception ex)
        {
            var errorMsg = $"{message}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[ERROR] {errorMsg}");
            
            StatusTitle!.Text = "Status: Error";
            StatusMessage!.Text = errorMsg;
        }
    }

    public class GuiSettings
    {
        public string SavePath { get; set; } = "";
        public string EmulatorPath { get; set; } = "";
        public bool AutoLaunch { get; set; } = true;
        public bool AutoSave { get; set; } = true;
    }
}