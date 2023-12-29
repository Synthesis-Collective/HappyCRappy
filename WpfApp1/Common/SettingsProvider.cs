using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class SettingsProvider
{
    private IEnvironmentStateProvider _environmentStateProvider;
    public SettingsProvider(IEnvironmentStateProvider environmentStateProvider)
    {
        _environmentStateProvider = environmentStateProvider;
        Settings = new();
        SettingsPath = Path.Combine(GetExePath(), "Settings", "Settings.json");
    }
    public HappyCrappySettings Settings { get; set; }
    public string SettingsPath { get; set; }

    private string GetExePath()
    {
        string? exeLocation = null;
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            exeLocation = Path.GetDirectoryName(assembly.Location);
            if (exeLocation == null)
            {
                throw new Exception("Located exe directory was null");
            }
            else
            {
                return exeLocation;
            }
        }
        else
        {
            throw new Exception("Could not locate running assembly");
        }
    }

    public void LoadSettings()
    {
        if (File.Exists(SettingsPath))
        {
            var settings = JSONhandler<HappyCrappySettings>.LoadJSONFile(SettingsPath, out bool success, out string _);
            if (success && settings != null)
            {
                Settings = settings;
            }
        }
    }

    public void SaveSettings()
    {
        IOFunctions.CreateDirectoryIfNeeded(SettingsPath, IOFunctions.PathType.File);
        JSONhandler<HappyCrappySettings>.SaveJSONFile(Settings, SettingsPath, out _, out _);
    }
}
