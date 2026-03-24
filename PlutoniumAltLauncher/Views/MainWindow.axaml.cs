using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Serilog;

namespace PlutoniumAltLauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppConfigManager.Load();

        //Init MainWindowBackgroundMusic
        InitBackgroundMusicHandling();
        
        //Init MainWindowInputHandler Gamepad handling
        InitGamepadHandling();
        
        //Disabling TAB and keyboard navigation on MainWindow
        //The logic with keys is custom, we don't this stuff
        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.None); 
        
        SystemDecorations = SystemDecorations.None;
        WindowState = WindowState.FullScreen;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }
    
    private void OpenSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        var win = new MainSettings();
        win.Show(this);
    }

    private void LaunchGame_OnClick(object? sender, RoutedEventArgs e)
    {
        var btn = (Button)sender!;
        var gameName = btn.Name!.ToLower().Replace("btn", "");
        
        var user = Environment.UserName;
        var plutoniumAppDataPath = $@"C:\Users\{user}\AppData\Local\Plutonium";
        var bootstrapperExecutable = $@"{plutoniumAppDataPath}\bin\plutonium-bootstrapper-win32.exe";
        var arguments = "";
        var exe = "";
        var gamePath = "";
        
        //Creating AppData dir if not present
        if (!Directory.Exists(plutoniumAppDataPath))
        {
            Directory.CreateDirectory(plutoniumAppDataPath);
            Log.Information("Created directory {PlutoniumAppDataPath}", plutoniumAppDataPath);
        }

        exe = bootstrapperExecutable;
        if (gameName == "plutonium") exe = AppConfigManager.Current.PlutoniumExecutablePath;
        else if (gameName.Contains("t4")) gamePath = AppConfigManager.Current.T4FolderPath;
        else if (gameName.Contains("t5")) gamePath = AppConfigManager.Current.T5FolderPath;
        else if (gameName.Contains("t6")) gamePath = AppConfigManager.Current.T6FolderPath;
        else arguments = gamePath = AppConfigManager.Current.IW5FolderPath;

        if (!string.IsNullOrEmpty(gamePath)) arguments = $"{gameName} {gamePath} +name {AppConfigManager.Current.IngameUsername} -lan"; 
        
        Log.Information("Executable {Executable}", exe);
        Log.Information("Arguments {Arguments}", arguments);
        Log.Information("WorkingDir {PlutoniumAppDataPath}", plutoniumAppDataPath);

        if (!ValidateButtonInput(gameName, gamePath, exe))
        {
            
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                Arguments = arguments,
                WorkingDirectory = plutoniumAppDataPath
            });
        }
        catch (Exception ex) { Log.Error(ex, "Process crashed :\\"); }
        
        Log.Information("Game launched successfully");
        

        if (AppConfigManager.Current.CloseAtLaunch)
        {
            Close();
            return;
        }
        
        ShowMessage("Wait please", "Launching...", 10, null);
    }

    private bool ValidateButtonInput(string gameName, string gamePath, string exe)
    {
        if (gameName != "plutonium" && string.IsNullOrEmpty(gamePath))
        {
            ShowMessage("⚠️ Invalid gamepath selected", "⚠️\nCheck your game paths in settings", 
                0, "Ok"
            );
            return false;
        }
        
        if (string.IsNullOrEmpty(exe))
        {
            ShowMessage(
                "⚠️ Invalid executable selected", "️⚠️\nCheck your plutonium.exe path in settings", 
                0, "Ok"
            );
            return false;
        }

        if (!File.Exists(exe))
        {
            ShowMessage(
                "⚠️ Plutonium installation not found", "️⚠️\nI can't find your Plutonium installation\nDid you boot the official launcher at least once?", 
                0, "Ok"
            );
            return false;
        }
        
        Log.Debug("Validation completed");
        return true;
    }
    
    private void ShowMessage(string title, string message, int timeout, string? confirmButtonMessage)
    {
        var win = new MessageWindow(title, message, timeout, confirmButtonMessage);
        win.Show(this);
    }
}