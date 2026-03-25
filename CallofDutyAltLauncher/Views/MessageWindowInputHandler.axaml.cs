using System;
using Avalonia.Threading;

namespace CallofDutyAltLauncher.Views;

public partial class MessageWindow
{
    //Init method invoked in MainWindow.axaml.cs
    private void InitGamepadHandling()
    {
        //New instance of gamepad, all handling is declared here
        _gamepad = new Gamepad();
        _gamepad.GamepadButtonPressed += OnGamepadButtonPressedOnMessageWindow;
        
        //When the window goes out of focus, tell the gamepad handler to stop polling for events
        //Activated and Deactivated are events fire by this Window
        Activated += (_, _) => _gamepad.StartGamepadHandling("MessageWindow");
        Deactivated += (_, _) => _gamepad.StopGamepadHandling();
    }

    private Gamepad _gamepad;
    
    private void OnGamepadButtonPressedOnMessageWindow(object? sender, string message)
    {
        var number = int.Parse(message);
        int[] newElementSelected = number switch
        {
            4 => [0, 0], //South
            5 => [0, 1], //East
            _ => [-1, -1]
        };
        
        //Pressed Unhandled button
        if (newElementSelected[0] == -1 && newElementSelected[1] == -1) return;
        
        //Pressed South or East
        if (newElementSelected[0] == -0 && (newElementSelected[1] == 0 || newElementSelected[1] == 1)) Dispatcher.UIThread.Post(Close);
    }
    
    //Stop gamepad thread when closing
    protected override void OnClosed(EventArgs e)
    {
        _gamepad.StopGamepadHandling();
        base.OnClosed(e);
    }
}