using System;
using System.Threading.Tasks;
using SDL3;

namespace CallofDutyAltLauncher;

public class Gamepad
{
    //Event sent to UI thread
    public event EventHandler<string>? GamepadButtonPressed;

    private bool _shouldBeKilled;

    public void StopGamepadHandling()
    {
        _shouldBeKilled = true;
    }


    public void StartGamepadHandling(string actor)
    {
        _shouldBeKilled = false;
        Task.Run(async () =>
        {
            SDL.Init(SDL.InitFlags.Gamepad);
            IntPtr gamepad = 0;
            
            //Up, down, left, right, south, east
            bool[] prevBtns = [true, true, true, true, true, true]; //Default to true, so when switching window you still have to release the button before registering a button pressed in this thread
            bool[] curBtns = [true, true, true, true, true, true];
            
            while (true)
            {
                if (_shouldBeKilled) return; //Completely stops polling if should be killed is set
                SDL.UpdateGamepads();
                if (!SDL.GamepadConnected(gamepad))
                {
                    //if (!string.IsNullOrEmpty(SDL.GetGamepadName(gamepad))) Console.WriteLine($"Gamepad {SDL.GetGamepadName(gamepad)} is now disconnected");
                    gamepad = await PollOpenGamepad();
                }
                
                curBtns[0] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.DPadUp);
                curBtns[1] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.DPadDown);
                curBtns[2] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.DPadLeft);
                curBtns[3] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.DPadRight);
                
                curBtns[4] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.South);
                curBtns[5] = SDL.GetGamepadButton(gamepad, SDL.GamepadButton.East);

                for (var i = 0; i < prevBtns.Length; i++)   //Button press is detected when previous it wasn't pressed and now it is
                {
                    if (!prevBtns[i] && curBtns[i])
                    {
                        GamepadButtonPressed?.Invoke(null, i.ToString());
                        //Console.WriteLine("Pressed " + i + " on " + actor);
                    }
                    prevBtns[i] = curBtns[i];
                }
                
                await Task.Delay(10);
            }
        });
    }

    
    private async Task<IntPtr> PollOpenGamepad()
    {
        while (true)
        {
            SDL.UpdateGamepads();
            // Background work here
            var gamepads = SDL.GetGamepads(out var count);
            if (gamepads is null || gamepads.Length == 0)
            {
                //Console.WriteLine("No gamepads found");
                await Task.Delay(5000);
                continue;
            }
            
            var gamepad = SDL.OpenGamepad(gamepads[0]);
            //Console.WriteLine($"Gamepad {SDL.GetGamepadName(gamepad)} is now connected ");
            return gamepad;
        }
    }
}