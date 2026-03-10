# PlutoniumAltLauncher 
#### *Plutonium Alternative Launcher*

A [Plutonium](https://plutonium.pw/) launcher that allows you to play while being offline, mainly developed with Steam Deck in mind and powered by [Avalonia](https://avaloniaui.net/).
It has controller, touchscreen, mouse/kb support and works on both Linux (through Wine/Proton) and Windows.

The main objective is giving a shortcut to play offline Call of Duty Plutonium-supported titles.

**Oh btw, I'm not affiliated with the Plutonium team in any capacity, show some gratitude to the devs!**

<img width="2559" height="1439" alt="immagine" src="https://github.com/user-attachments/assets/02d00751-f51d-4982-bcec-ba0b18f495fe" />

## Requirements
- Call of Duty game files
- [Plutonium](https://cdn.plutonium.pw/updater/plutonium.exe)

## How to install
1. Download and extract the `PlutoniumAltLauncher.zip` from [Releases](https://github.com/framilano/PlutoniumDeckLauncher/releases)
2. Here's my folder structure in `/home/deck/NoLauncherGames/Plutonium`:

```
├── PlutoniumWAW    # Contains World at War game files
├── PlutoniumBO1    # Contains Black Ops 1 game files
├── PlutoniumBO2    # Contains Black Ops 2 game files
├── PlutoniumIW5    # Contains MW3 game files
├── plutonium.exe   # Plutonium executable downloaded from Plutonium site
└── PlutoniumAltLauncher
    ├── Assets
    ├── config.json
    └── PlutoniumAltLauncher.exe
```
You can change this folder structure editing the `config.json` file.

3. Add `PlutoniumAltLauncher.exe` as non-steam game in `Desktop Mode`.
4. Set `Proton Experimental` or `Proton-GE` in compatibility settings, `Proton GE` is recommended.
5. Add the following line (if you're on kernel < 6.14 or without the NTSync module loaded) on command launch arguments (fsync and esync are known to cause issues with BO1/2):

   `PROTON_NO_ESYNC=1 PROTON_NO_FSYNC=1 %command%`

6. With NTSYNC on kernel >= 6.14 and `Proton-GE` integrating it by default there's no need for these launch arguments anymore. **With Steam Deck update 3.7.20 now NTSYNC is available!**.
7. Start the program, you need to first boot the classic plutonium launcher (so it can download required files inside the proton prefix). Close everything and reboot the launcher, you're done!

## Extra
- You need to install through `Protontricks` the `xact` audio library to have all Sound FXs on BO1 and WAW
- You can edit `config.json` from the program itself when clicking/touching on the settings icon on the top right of the screen. You'll probably need a mouse or trackpads for better navigation in settings.
- It's my first dotnet project, have mercy
- Why a Windows launcher and not native Linux? Because I wanted to simplify the whole configuration process, add PlutoniumAltLauncher.exe as non steam game, that's it, no looking for Proton versions, no different prefix for Plutonium itself. Just a single prefix with everything related to Plutonium.

## Configuration

There are some editable fields in `config.json`:
- `PlutoniumExecutablePath` set your plutonium.exe path
- `IngameUsername` set your ingame username while playing offline
- `T4/5/6/IW5 FolderPath` set your game files path
- `CloseAtLaunch` exit the launcher when booting the game, or leave it in background so you can open it back later
- `DisableBackgroundMusic` disable the background music immediately

## Building
```
dotnet publish -c Release -r win-x64 --self-contained true
robocopy PlutoniumAltLauncher/Native/libvlc PlutoniumAltLauncher/bin/Release/net10.0/win-x64/publish/libvlc /MIR
```

## Todo
- [X] Add Controller support
- [X] Play background music
- [X] Set IW5SP image
- [X] Replace images with higher resolution ones? Meh
- [X] Stop music when launching Plutonium
- [X] Option to close Launcher automatically when launching Plutonium
- [X] Change music (and loop it) when changing game in main menu
- [X] Check if player started Plutonium at least once
- [ ] Replace that horrible settings icon
- [ ] Better UI in settings, controller navigation?

## Issues
- Blank screen on some windows randomly while using Gamescope, just moving the pointer fixes it, weird
- Black screen with Mangohud enabled on Gamescope
