using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Serilog;

namespace PlutoniumAltLauncher.Views;

public partial class MainWindow
{

    private int[] CurrentElementSelected { get; set; } = [0, 0];
    private readonly Dictionary<string, string> _elementsDict = new Dictionary<string, string>
    {
        ["0;0"] = "PlutoniumBtn",
        ["1;0"] = "Iw4XBtn",
        ["0;1"] = "T4SpBtn",
        ["1;1"] = "T4MpBtn",
        ["0;2"] = "T5SpBtn",
        ["1;2"] = "T5MpBtn",
        ["0;3"] = "T6ZmBtn",
        ["1;3"] = "T6MpBtn",
        ["0;4"] = "Iw5SpBtn",
        ["1;4"] = "Iw5MpBtn",
        ["0;5"] = "Iw4SpBtn",
        ["1;5"] = "Iw4MpBtn",
    };
    
    private void AddHighlight(Button element)
    {
        
        //Telling the backgroundmusic to change song
        ChangeMusic(element.Name!.ToLower().Replace("btn", ""));
        
        foreach (var keyPair in _elementsDict)
        {   
            //Remove highlight from every element when adding a new highlight (could be useful is a cursor is in window and we're
            //moving with a gamepad
            RemoveHighlight(this.FindControl<Button>(_elementsDict[keyPair.Key])!);
        }
        
        element.Opacity = 1;
    }

    private static void RemoveHighlight(Button element)
    {
        element.Opacity = 0.2;
    }

    private void ComputeHighlightedElement(int input)
    {
        int[] newElementSelected = input switch
        {
            0 => [CurrentElementSelected[0], CurrentElementSelected[1] - 1],
            1 => [CurrentElementSelected[0], CurrentElementSelected[1] + 1],
            2 => [CurrentElementSelected[0] - 1, CurrentElementSelected[1]],
            3 => [CurrentElementSelected[0] + 1, CurrentElementSelected[1]],
            4 => [-1, -1], //South
            5 => [-1, -2], //East
            _ => [-2, -2]
        };
        
        //Pressed Unhandled button
        if (newElementSelected[0] == -2 && newElementSelected[1] == -2) return;
        
        //Pressed South
        if (newElementSelected[0] == -1 && newElementSelected[1] == -1)
        {
            Dispatcher.UIThread.Post(() =>
            {
                //Emulating click event
                var currentHighlightedButton =
                    this.FindControl<Button>(
                        _elementsDict[$"{CurrentElementSelected[0]};{CurrentElementSelected[1]}"])!;
                currentHighlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            });
            Log.Information("Clicked on button!");
            return;
        }
        
        //Pressed East
        if (newElementSelected[0] == -1 && newElementSelected[1] == -2) Dispatcher.UIThread.Post(Close);
        
        //ContentScrollViewer.LineDown();
        
        //Pressed DPAD or Key directions
        if (newElementSelected[0] < 0 ||
            newElementSelected[0] > 1 ||
            newElementSelected[1] < 0 ||
            newElementSelected[1] > 5)
        {
            Log.Information("Value outside of bounds, ignoring");
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            var currentHighlightedButton =
                this.FindControl<Button>(_elementsDict[$"{CurrentElementSelected[0]};{CurrentElementSelected[1]}"])!;
            RemoveHighlight(currentHighlightedButton);

            CurrentElementSelected = newElementSelected;
            
            //Scroll Down
            if (input == 1) ContentScrollViewer.Offset = ContentScrollViewer.Offset.WithY(
                ContentScrollViewer.Offset.Y + ContentScrollViewer.Viewport.Height * 0.2
            );
            
            //Scroll Up
            if (input == 0) ContentScrollViewer.Offset = ContentScrollViewer.Offset.WithY(
                ContentScrollViewer.Offset.Y - ContentScrollViewer.Viewport.Height * 0.2
            );


            var buttonToHighlight =
                this.FindControl<Button>(_elementsDict[$"{CurrentElementSelected[0]};{CurrentElementSelected[1]}"])!;
            AddHighlight(buttonToHighlight);
            //ButtonViewer.Text = message;
        });
    }
    
    /****************** MOUSE HANDLING ******************/

    private void OnPointerEntered(object? sender, RoutedEventArgs _)
    {
        if (!IsActive) return;  //Ignore if window is not active
        
        var btn = (Button)sender!;
        var name = btn.Name!;
        
        var enteredButton = this.FindControl<Button>(name)!;
        AddHighlight(enteredButton);

        foreach (var element in _elementsDict)
        {
            if (element.Value != name) continue;
            CurrentElementSelected[0] = int.Parse(element.Key.Split(";")[0]);
            CurrentElementSelected[1] = int.Parse(element.Key.Split(";")[1]);
        }
    }
    
    private void OnPointerExited(object? sender, RoutedEventArgs _)
    {
        if (!IsActive) return;  //Ignore if window is not active

        var btn = (Button)sender!;
        var name = btn.Name!;
        
        var enteredButton = this.FindControl<Button>(name)!;
        RemoveHighlight(enteredButton);
    }
    
    /****************** KEYBOARD HANDLING *****************/
    
    private void OnKeyDown(object? _, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
        
        var direction = e.Key switch //These numbers are the same I use for DPAD conversion to number
        {
            Key.Up => 0,
            Key.Down => 1,
            Key.Left => 2,
            Key.Right => 3,
            Key.Enter => 4,
            _ => -1
        };

        ComputeHighlightedElement(direction);
    }
    
    /****************** GAMEPAD HANDLING ******************/
    
    //Init method invoked in MainWindow.axaml.cs
    private void InitGamepadHandling()
    {
        //New instance of gamepad, all handling is declared here
        _gamepad = new Gamepad();
        _gamepad.GamepadButtonPressed += OnGamepadButtonPressedOnMainWindow;
        
        //When the window goes out of focus, tell the gamepad handler to stop polling for events
        //Activated and Deactivated are events fired by this Window
        Activated += (_, _) =>
        {
            //Console.WriteLine("Enabled gamepad on MainWindow");
            _gamepad.StartGamepadHandling("MainWindow");
        };
        Deactivated += (_, _) =>
        {
            //Console.WriteLine("Disabled gamepad on MainWindow");
            _gamepad.StopGamepadHandling();
        };
    }

    private Gamepad _gamepad;
    
    private void OnGamepadButtonPressedOnMainWindow(object? _, string message)
    {
        ComputeHighlightedElement(int.Parse(message));
    }
    
    //Stop gamepad and music threads when closing
    protected override void OnClosed(EventArgs e)
    {
        //Could be null if user decided to disable background music
        if (_backgroundMusic is not null) _backgroundMusic!.StopBackgroundMusicHandling();
        _gamepad.StopGamepadHandling();
        base.OnClosed(e);
    }
}