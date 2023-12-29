using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_IO
{
    private readonly SettingsProvider _settingsProvider;
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    public VM_IO(SettingsProvider settingsProvider, IEnvironmentStateProvider environmentStateProvider)
    {
        _settingsProvider = settingsProvider;
        _environmentStateProvider = environmentStateProvider;
    }

    public void CopyInViewModels()
    {
        _settingsProvider.LoadSettings();
        SetEnvironmentState();
        // load in view models here
    }

    public void DumpViewModels()
    {
        DumpEnvironmentState();
        _settingsProvider.SaveSettings();
        // dump view models to models here
    }

    private void SetEnvironmentState()
    {
        if (_environmentStateProvider is StandaloneEnvironmentStateProvider standaloneProvider)
        {
            standaloneProvider.SetGameType(_settingsProvider.Settings.GameType);
            standaloneProvider.SetDataFolder(_settingsProvider.Settings.DataFolderPath);
        }
    }

    private void DumpEnvironmentState()
    {
        _settingsProvider.Settings.GameType = _environmentStateProvider.GameType;
        _settingsProvider.Settings.DataFolderPath = _environmentStateProvider.DataFolderPath;
    }
}
