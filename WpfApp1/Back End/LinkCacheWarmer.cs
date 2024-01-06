using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace HappyCRappy;

public class LinkCacheWarmer : VM
{
    public LinkCacheWarmer(IEnvironmentStateProvider environmentStateProvider, VM_SettingsMenu settingsMenu, VM_SnapshotMenu snapshotMenu, RecordUtils recordUtils)
    {
        _environmentStateProvider = environmentStateProvider;
        _settingsMenu = settingsMenu;
        _snapshotMenu = snapshotMenu;
        _recordUtils = recordUtils;
    }
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private readonly VM_SettingsMenu _settingsMenu;
    private readonly VM_SnapshotMenu _snapshotMenu;
    private readonly RecordUtils _recordUtils;

    private List<ModKey> WarmedModKeys { get; set; } = new();
    
    public void InitializeSubscriptions()
    {
        this.WhenAnyValue(x => x._settingsMenu.WarmUpLinkCacheOnStartup)
            .Skip(1)
            .Subscribe(bWarmUp =>
            {
                if (bWarmUp)
                {
                    Task.Run(async () =>
                    {
                        await WarmUpLinkCache();
                    });
                }
            })
            .DisposeWith(this);

        _settingsMenu.TrackedModKeys
            .ToObservableChangeSet()
            .Skip(1)
            .Subscribe(_ =>
            {
                if (_settingsMenu.WarmUpLinkCacheOnStartup)
                {
                    Task.Run(async () =>
                    {
                        await WarmUpLinkCache();
                    });
                }
            })
            .DisposeWith(this);
    }

    public async Task WarmUpLinkCache()
    {
        foreach (var modKey in _settingsMenu.TrackedModKeys)
        {
            if (WarmedModKeys.Contains(modKey))
            {
                continue;
            }
            else
            {
                WarmedModKeys.Add(modKey);
            }

            if(_snapshotMenu.ShowPotentialConflicts)
            {
                (var overrideRecords, var overrideRecordMasters) = _recordUtils.GetModOverriddenRecords(modKey);
                var nonMasters = overrideRecords.Where(x => !overrideRecordMasters.Contains(x)).ToList();
                var modsToWarmUp = new List<ModKey>();
                if (_settingsMenu.HandleRemappedFormTypes)
                {
                    modsToWarmUp = overrideRecords;
                }
                else
                {
                    modsToWarmUp = nonMasters;
                }
                foreach (var overrideMod in modsToWarmUp)
                {
                    _recordUtils.ResolveAllRecordContexts(overrideMod);
                }
                bool debug = false;
            }
            else
            {
                _recordUtils.GetMasterRecords(modKey); 
            }
        }
    }
}
