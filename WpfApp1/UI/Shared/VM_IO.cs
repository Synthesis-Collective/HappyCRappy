using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_IO
{
    private readonly SettingsProvider _settingsProvider;
    private readonly VM_SettingsMenu _settingsMenu;
    public VM_IO(SettingsProvider settingsProvider, VM_SettingsMenu settingsMenu)
    {
        _settingsProvider = settingsProvider;
        _settingsMenu = settingsMenu;
    }

    public void CopyInViewModels()
    {
        _settingsProvider.LoadSettings();
        _settingsMenu.ReadFromModel(_settingsProvider.Settings);
        // load in view models here
    }

    public void DumpViewModels()
    {
        _settingsProvider.Settings = _settingsMenu.DumpToModel();
        _settingsProvider.SaveSettings();
        // dump view models to models here
    }
}
