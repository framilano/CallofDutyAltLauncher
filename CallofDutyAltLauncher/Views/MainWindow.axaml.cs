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
        var workingDir = $@"C:\Users\{user}\AppData\Local\Plutonium";
        var arguments = "";
        var exe = "";
        var gamePath = "";
        
        //Creating AppData dir if not present
        if (!Directory.Exists(workingDir))
        {
            Directory.CreateDirectory(workingDir);
            Log.Information("Created directory {WorkingDir}", workingDir);
        }

        if (gameName == "plutonium")
        {
            exe = AppConfigManager.Current.PlutoniumExecutablePath;
        }
        else if (gameName.Contains("t4"))
        {
            exe = $@"{workingDir}\bin\plutonium-bootstrapper-win32.exe";
            gamePath = AppConfigManager.Current.T4FolderPath;
            arguments = $"{gameName} {gamePath} -lan +name {AppConfigManager.Current.IngameUsername}";
        }
        else if (gameName.Contains("t5"))
        {
            exe = $@"{workingDir}\bin\plutonium-bootstrapper-win32.exe";
            gamePath = AppConfigManager.Current.T5FolderPath;
            arguments = $"{gameName} {gamePath} -lan +name {AppConfigManager.Current.IngameUsername}";
        }
        else if (gameName.Contains("t6"))
        {
            exe = $@"{workingDir}\bin\plutonium-bootstrapper-win32.exe";
            gamePath = AppConfigManager.Current.T6FolderPath;
            arguments = $"{gameName} {gamePath} -lan +name {AppConfigManager.Current.IngameUsername}";
        }
        else if (gameName.Contains("iw5"))
        {
            exe = $@"{workingDir}\bin\plutonium-bootstrapper-win32.exe";
            gamePath = AppConfigManager.Current.IW5FolderPath;
            arguments = $"{gameName} {gamePath} -lan +name {AppConfigManager.Current.IngameUsername}";
        }
        else if (gameName == "iw4x")
        {
            workingDir = AppConfigManager.Current.IW4FolderPath;
            exe = workingDir + "iw4x-launcher.exe";
            gamePath = AppConfigManager.Current.IW4FolderPath;
            arguments = $"--path {gamePath} ";
        }
        else if (gameName == "iw4mp")
        {
            workingDir = AppConfigManager.Current.IW4FolderPath;
            exe = workingDir + "iw4x.exe";
            gamePath = AppConfigManager.Current.IW4FolderPath;
            arguments = $"+sv_securityLevel 0 +name {AppConfigManager.Current.IngameUsername}"; //SecurityLevel 0 allows same machine instance (for splitscreen programs)
        }
        
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
        
        //No game cannot have an exe
        if (string.IsNullOrEmpty(exeName))
        {
            ShowMessage("⚠️ No game executable found ⚠️\n", "⚠️ No game executable found ⚠️\nCheck your game paths in settings", 
                0, "Ok"
            );
            return false;
        }
        
        //Only online Plutonium can have an empty game path
        if (gameName != "plutonium" && string.IsNullOrEmpty(gamePath))
        {
            ShowMessage("⚠️ Invalid gamepath selected ⚠️\n", "⚠️ Invalid gamepath selected ⚠️\nCheck your game paths in settings", 
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
                   message = "⚠️ I can't find \"plutonium.exe\" ⚠️\nCheck your Plutonium path in settings";
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