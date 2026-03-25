using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace CallofDutyAltLauncher.Views;

public partial class MessageWindow : Window
{
    public MessageWindow(string title, string message, int timeout, string? confirmButtonMessage)
    {
        InitializeComponent();
        Title = title;
        Message.Text = message;

        if (confirmButtonMessage is not null)
        {
            ConfirmButton.IsVisible = true;
            ConfirmButton.Content = confirmButtonMessage;
        }
        
        //Init MessageWindowGamepad
        InitGamepadHandling();
        
        //Destroy window with timer if timeout is different than 0
        if (timeout != 0) Task.Run(async () =>
        {
            await Task.Delay(timeout * 1000);
            
            Dispatcher.UIThread.Post(Close);
        });
        
        
        SystemDecorations = SystemDecorations.None;
        WindowState = WindowState.FullScreen;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void Quit(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}