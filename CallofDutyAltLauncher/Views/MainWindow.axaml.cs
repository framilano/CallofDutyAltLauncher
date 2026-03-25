using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Serilog;

namespace CallofDutyAltLauncher.Views;

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
        var plutoniumBootstraperExecutable = $@"{plutoniumAppDataPath}\bin\plutonium-bootstrapper-win32.exe";
        var workingDir = plutoniumAppDataPath;
        var arguments = "";
        var exe = "";
        var gamePath = "";
        
        //Creating AppData dir if not present
        if (!Directory.Exists(plutoniumAppDataPath))
        {
            Directory.CreateDirectory(plutoniumAppDataPath);
            Log.Information("Created directory {PlutoniumAppDataPath}", plutoniumAppDataPath);
        }

        exe = plutoniumBootstraperExecutable;
        if (gameName == "plutonium") exe = AppConfigManager.Current.PlutoniumExecutablePath;
        else if (gameName.Contains("t4")) gamePath = AppConfigManager.Current.T4FolderPath;
        else if (gameName.Contains("t5")) gamePath = AppConfigManager.Current.T5FolderPath;
        else if (gameName.Contains("t6")) gamePath = AppConfigManager.Current.T6FolderPath;
        else if (gameName.Contains("iw5")) gamePath = AppConfigManager.Current.IW5FolderPath;
        else if (gameName == "iw4x")
        {
            gamePath = AppConfigManager.Current.IW4FolderPath;
            workingDir = AppConfigManager.Current.IW4FolderPath;
            exe = workingDir + "iw4x-launcher.exe";
        }
        else if (gameName == "iw4mp")
        {
            gamePath = AppConfigManager.Current.IW4FolderPath; 
            workingDir = AppConfigManager.Current.IW4FolderPath;
            exe = workingDir + "iw4x.exe";
        }

        if (!string.IsNullOrEmpty(gamePath)) arguments = $"{gameName} {gamePath} -lan +name {AppConfigManager.Current.IngameUsername}"; 
        
        Log.Information("Executable {Executable}", exe);
        Log.Information("Arguments {Arguments}", arguments);
        Log.Information("WorkingDir {WorkingDir}", workingDir);

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
                WorkingDirectory = workingDir
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

        var exeName = exe.Contains('\\') ? exe.Split('\\').Last() : exe.Split('/').Last();
        
        //Only online Plutonium can have an empty game path
        if (gameName != "plutonium" && string.IsNullOrEmpty(gamePath))
        {
            ShowMessage("⚠️ Invalid gamepath selected", "⚠️\nCheck your game paths in settings", 
                0, "Ok"
            );
            return false;
        }

        if (!File.Exists(exe))
        {
            var title = ""; 
            var message = "";
            switch (exeName)
            {
               case "plutonium-bootstrapper-win32.exe":
               {
                   title = "⚠️ Plutonium installation not found ⚠️\n";
                   message = "I can't find your Plutonium installation\nDid you boot the official launcher at least once?";
                   break;
               }
               case "plutonium.exe":
               {
                   title = "⚠️ I can't find \"plutonium.exe\" ⚠️\n";
                   message = "Check your Plutonium path in settings";
                   break;
               }
               case "iw4x.exe":
               {
                   title = "⚠️ I can't find \"iw4x.exe\" ⚠️\n";
                   message = "I can't find your IW4x executable\nDid you boot the online IW4x launcher at least once?";
                   break;
               }
               case "iw4x-launcher.exe":
               {
                   title = "⚠️  I can't find \"iw4x-launcher.exe\" ⚠️\n";
                   message = "I can't find your IW4x Launcher executable\nDid you place the launcher in your MW2 game folder?";
                   break;
               }
            }
            
            ShowMessage(title, message, 
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