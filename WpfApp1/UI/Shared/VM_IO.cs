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
    private readonly VM_SnapshotMenu _snapshotMenu;
    public VM_IO(SettingsProvider settingsProvider, VM_SettingsMenu settingsMenu, VM_SnapshotMenu snapshotMenu)
    {
        _settingsProvider = settingsProvider;
        _settingsMenu = settingsMenu;
        _snapshotMenu = snapshotMenu;
    }

    public void CopyInViewModels()
    {
        _settingsProvider.LoadSettings();
        _settingsMenu.ReadFromModel(_settingsProvider.Settings);
        _snapshotMenu.ReadFromModel(_settingsProvider.Settings);
    }

    public void DumpViewModels()
    {
        _settingsProvider.Settings = _settingsMenu.DumpToModel();
        _settingsProvider.SaveSettings();
        _snapshotMenu.SaveSettings();
    }
}
