using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace HappyCRappy;

public class ModRecordListing
{
    public delegate ModRecordListing Factory(ModKey modKey);
    public ModRecordListing(ModKey modKey, IEnvironmentStateProvider environmentStateProvider)
    {
        _environmentStateProvider = environmentStateProvider;
        ModKey = modKey;

        Task.Run(async () =>
        {
            await Initialize();
        });
    }

    public ModKey ModKey { get; set; }
    public HashSet<IMajorRecordGetter> Records { get; set; } = new();
    public bool Initialized { get; set; } = false;
    private readonly IEnvironmentStateProvider _environmentStateProvider;

    public async Task Initialize()
    {
        var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(ModKey);
        if (modListing == null)
        {
            return;
        }

        Records = modListing?.Mod?.EnumerateMajorRecords()?.ToHashSet() ?? new();
        Initialized = true;
    }
}
