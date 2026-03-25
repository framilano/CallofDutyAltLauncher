using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace CallofDutyAltLauncher.Views;

public partial class MainSettings : Window
{
    public MainSettings()
    {
        InitializeComponent();
        
        PlutoniumPath.Text =  AppConfigManager.Current.PlutoniumExecutablePath;
        IngameUsername.Text = AppConfigManager.Current.IngameUsername;
        T4FolderPath.Text = AppConfigManager.Current.T4FolderPath;
        T5FolderPath.Text = AppConfigManager.Current.T5FolderPath;
        T6FolderPath.Text = AppConfigManager.Current.T6FolderPath;
        IW5FolderPath.Text = AppConfigManager.Current.IW5FolderPath;
        IW4FolderPath.Text = AppConfigManager.Current.IW4FolderPath;
        CloseAtLaunch.IsChecked = AppConfigManager.Current.CloseAtLaunch;
        DisableBackgroundMusic.IsChecked = AppConfigManager.Current.DisableBackgroundMusic;
        
        SystemDecorations = SystemDecorations.None;
        WindowState = WindowState.FullScreen;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    }
    
    private async Task<string> SelectFile()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select executable",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Executable")
                {
                    Patterns = new[] { "*.exe" }
                }
            }
        });

        if (files.Count > 0)
        {
            var file = files[0];
            return file.Path.LocalPath;
        }

        return "";
    }
    
    private async Task<string> SelectFolder()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select folder",
            AllowMultiple = false,
        });

        if (folders.Count > 0)
        {
            var folder = folders[0];
            return folder.Path.LocalPath;
        }

        return "";
    }

    private async void SelectFilePlutoniumPath_OnClick(object? sender, RoutedEventArgs e)
    {
        var btn = (Button)sender!;
        var pathName = btn.Name!.ToLower().Replace("btn", "");

        if (pathName == "plutoniumpath")
        {
            var plutoniumPath = await SelectFile();
            PlutoniumPath.Text = plutoniumPath;
            AppConfigManager.Current.PlutoniumExecutablePath = plutoniumPath;
        }
    }
    
    private async void SelectGameFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        var btn = (Button)sender!;
        var tag = btn.Tag!.ToString();
        var folder = await SelectFolder();
        switch (tag)
        {
            case "T4FolderButton":
            {
                T4FolderPath.Text = folder;
                AppConfigManager.Current.T4FolderPath = folder;
                break;
            }
            case "T5FolderButton":
            {
                T5FolderPath.Text = folder;
                AppConfigManager.Current.T5FolderPath = folder;
                break;
            }
            case "T6FolderButton":
            {
                T6FolderPath.Text = folder;
                AppConfigManager.Current.T6FolderPath = folder;
                break;
            }
            case "IW5FolderButton":
            {
                IW5FolderPath.Text = folder;
                AppConfigManager.Current.IW5FolderPath = folder;
                break;
            }
            case "IW4FolderButton":
            {
                IW4FolderPath.Text = folder;
                AppConfigManager.Current.IW4FolderPath = folder;
                break;
            }
        }
    }
    
    private async void CloseLauncher_OnCheck(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        AppConfigManager.Current.CloseAtLaunch = checkBox.IsChecked!.Value;
    }
    
    private void DisableBackgroundMusic_OnCheck(object? sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender!;
        AppConfigManager.Current.DisableBackgroundMusic = checkBox.IsChecked!.Value;
    }

    private void SaveConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        AppConfigManager.Current.PlutoniumExecutablePath = PlutoniumPath.Text!;
        AppConfigManager.Current.IngameUsername = IngameUsername.Text!;
        AppConfigManager.Current.T4FolderPath = T4FolderPath.Text!;
        AppConfigManager.Current.T5FolderPath = T5FolderPath.Text!;
        AppConfigManager.Current.T6FolderPath = T6FolderPath.Text!;
        AppConfigManager.Current.IW5FolderPath = IW5FolderPath.Text!;
        AppConfigManager.Current.IW4FolderPath = IW4FolderPath.Text!;
        AppConfigManager.Current.CloseAtLaunch = CloseAtLaunch.IsChecked!.Value;
        AppConfigManager.Current.DisableBackgroundMusic = DisableBackgroundMusic.IsChecked!.Value;
        AppConfigManager.Save();
        Close(); //Closes window
    }
}