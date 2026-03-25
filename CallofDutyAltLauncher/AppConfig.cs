using System;
using System.Text.Json.Serialization;
using Serilog;
using System.IO;
using System.Text.Json;

namespace CallofDutyAltLauncher;


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfigManager.AppConfig))]
internal partial class SourceGenerationContext : JsonSerializerContext { }

public static class AppConfigManager
{
    private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

    
    public class AppConfig
    {
        public string PlutoniumExecutablePath { get; set; } = "";
        public string IngameUsername { get; set; } = "offlineuser";
        public string T4FolderPath { get; set; } = "";
        public string T5FolderPath { get; set; } = "";
        public string T6FolderPath { get; set; } = "";
        public string IW5FolderPath { get; set; } = "";
        public string IW4FolderPath { get; set; } = "";
        public bool   CloseAtLaunch { get; set; }
        public bool   DisableBackgroundMusic { get; set; }
    }
    
    public static AppConfig Current { get; set; } = new();

    public static void Save()
    {
        var json = JsonSerializer.Serialize(Current, SourceGenerationContext.Default.AppConfig);
        Log.Information("Saved settings {Current}", json);
        File.WriteAllText(ConfigPath, json);
    }
    
    public static void Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (!File.Exists(path))
        {
            Log.Information("config.json is missing, creating a new AppConfig, delaying file creation until first saving");
            Current = new AppConfig(); // default values
            return;
        }

        var json = File.ReadAllText(path);
        
        Log.Information("Loaded settings {Current}", json);
        var appConfig = JsonSerializer.Deserialize<AppConfig>(json, SourceGenerationContext.Default.AppConfig) ?? new AppConfig();
        Current = appConfig;
    }
}