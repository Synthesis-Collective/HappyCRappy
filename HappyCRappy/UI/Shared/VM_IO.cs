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
    private readonly LinkCacheWarmer _linkCacheWarmer;
    private readonly VM_LoadOrderMenu _loadOrderMenu;
    public VM_IO(SettingsProvider settingsProvider, VM_SettingsMenu settingsMenu, VM_SnapshotMenu snapshotMenu, VM_LoadOrderMenu loadOrderMenu, LinkCacheWarmer linkCacheWarmer)
    {
        _settingsProvider = settingsProvider;
        _settingsMenu = settingsMenu;
        _snapshotMenu = snapshotMenu;
        _loadOrderMenu = loadOrderMenu;
        _linkCacheWarmer = linkCacheWarmer; 
    }

    public void CopyInViewModels()
    {
        _settingsProvider.LoadSettings();
        _settingsMenu.ReadFromModel(_settingsProvider.Settings);
        _snapshotMenu.ReadFromModel(_settingsProvider.Settings);

        if (_settingsMenu.WarmUpLinkCacheOnStartup)
        {
            Task.Run(async () =>
            {
                await _linkCacheWarmer.WarmUpLinkCache();
            });
        }

        _linkCacheWarmer.InitializeSubscriptions();
    }

    public void DumpViewModels()
    {
        _settingsProvider.Settings = _settingsMenu.DumpToModel();
        _settingsProvider.SaveSettings();
        _snapshotMenu.SaveSettings();
    }
}
