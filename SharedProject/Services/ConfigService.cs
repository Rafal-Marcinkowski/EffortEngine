using SharedProject.Models;
using System.IO;
using System.Text.Json;

namespace SharedProject.Services;

public class ConfigService
{
    private const string ConfigFileName = "pomodoro.config";
    private readonly string configPath;

    public ConfigService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "EffortEngine");
        Directory.CreateDirectory(appFolder);
        configPath = Path.Combine(appFolder, ConfigFileName);
    }

    public PomodoroConfig LoadConfig()
    {
        try
        {
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<PomodoroConfig>(json) ?? new PomodoroConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
        }
        return new PomodoroConfig();
    }

    public void SaveConfig(PomodoroConfig config)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config: {ex.Message}");
        }
    }
}