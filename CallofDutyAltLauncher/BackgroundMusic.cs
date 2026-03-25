using System;
using System.IO;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace CallofDutyAltLauncher;

public class BackgroundMusic
{
    private string? SongToPlay { get; set; } = "online.mp3";

    private MediaPlayer? _player;
    
    private bool _shouldBeKilled;
    
    private readonly LibVLC _libVlc = new();

    public void StopBackgroundMusicHandling()
    {
        _shouldBeKilled = true;
    }
    
    public void ChangeSong(string songName)
    {
        if (songName == SongToPlay) return;
        SongToPlay = songName;
    }
    
    public void StartBackgroundMusicHandling()
    {
        _shouldBeKilled = false;
        Task.Run(async () =>
        {
            Core.Initialize(); // loads native VLC

            //Wait half a second before starting, we just started the program
            //and maybe a mouse cursor is being placed on a different position than top center.
            //So we don't get this horrible effect at launch: start online.mp3, immediately stops, start another
            await Task.Delay(500);
            
            while (true)
            {
                if (_shouldBeKilled) return;    //Window is out of focus or background music is disabled
                await PlayMusic();
            }
        });
    }

    private async Task PlayMusic()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets/music", SongToPlay!);
        var playingSong = SongToPlay!;
        using var media = new Media(_libVlc, path);
        _player = new MediaPlayer(media);

        _player.Play();
        //_player.IsPlaying is too slow, using a variable to check if we're playing or not
        var realIsPlaying = true; 
        _player.Volume = 35;
        
        //Console.WriteLine("Playing...");
        
        //if the SongToPlay has changed, exit and restart the background music logic
        while (realIsPlaying && !_shouldBeKilled && SongToPlay == playingSong)
        {
            await Task.Delay(100);
            realIsPlaying = _player.IsPlaying;  //Now we align the fake and real isPlaying value
        }
        _player.Stop();
    }
}