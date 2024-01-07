using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using System.Windows;
using Loqui;

namespace HappyCRappy;

public class RecordUtils
{
    public RecordUtils(IEnvironmentStateProvider environmentStateProvider, VM_SettingsMenu settingsMenu, ModRecordListing.Factory listingFactory)
    {
        _environmentStateProvider = environmentStateProvider;
        _settingsMenu = settingsMenu;
        _listingFactory = listingFactory;
    }
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private readonly VM_SettingsMenu _settingsMenu;
    readonly ModRecordListing.Factory _listingFactory;
    private HashSet<ModRecordListing> _modRecords { get; set; } = new();

    public IMajorRecordGetter[]? GetModRecords(ModKey modKey)
    {
        if(_settingsMenu.UseDeepCacheing)
        {
            return GetModRecordsFromDeepCache(modKey);
        }
        else
        {
            var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(modKey);
            var records = modListing?.Mod?.EnumerateMajorRecords()?.ToHashSet() ?? new();
            return records.ToArray();
        }
    }

    private IMajorRecordGetter[]? GetModRecordsFromDeepCache(ModKey modKey)
    {
        var modRecordListing = _modRecords.Where(x => x.ModKey.Equals(modKey)).FirstOrDefault();
        if (modRecordListing == null)
        {
            modRecordListing = _listingFactory(modKey);
            _modRecords.Add(modRecordListing);
        }

        while (!modRecordListing.Initialized)
        {
            continue;
        }

        return modRecordListing.Records.ToArray();
    }

    public List<ModKey>? GetMasterRecords(ModKey modKey)
    {
        var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(modKey);
        if (modListing == null || modListing.Mod == null)
        {
            System.Windows.MessageBox.Show("Could not find mod " + modKey.ToString() + " in current load order", "Error");
            return null;
        }

        return modListing.Mod.MasterReferences.Select(x => x.Master).ToList();
    }

    public (List<ModKey>, List<ModKey>) GetModOverriddenRecords(ModKey modKey) // returns the mods that are overridden by the given mod, as well as a list of these mods' masters (to enable downstream filtering)
    {
        var records = GetModRecords(modKey);

        List<ModKey> overriddenMods = new();
        List<ModKey> overriddenModMasters = new();

        if (records == null)
        {
            return (overriddenMods, overriddenModMasters);
        }

        foreach (var record in records)
        {
            var registration = LoquiRegistration.StaticRegister.GetRegister(record.GetType());
            if (registration != null)
            {
                var contexts = _environmentStateProvider.LinkCache?.ResolveAllContexts(record.FormKey, registration.GetterType).ToList() ?? new();
                foreach (var context in contexts)
                {
                    if (context.ModKey.Equals(modKey))
                    {
                        continue;
                    }

                    if (!overriddenMods.Contains(context.ModKey))
                    {
                        overriddenMods.Add(context.ModKey);
                        var masters = GetMasterRecords(context.ModKey);
                        if (masters != null)
                        {
                            overriddenModMasters.AddRange(masters.Where(x => !overriddenModMasters.Contains(x)));
                        }
                    }
                }
            }
        }

        return (overriddenMods, overriddenModMasters);
    }

    public void ResolveAllRecordContexts(ModKey modKey)
    {
        var records = GetModRecords(modKey);
        if (records == null || _environmentStateProvider.LinkCache == null)
        {
            return;
        }

        foreach (var record in records)
        {
            var registration = LoquiRegistration.StaticRegister.GetRegister(record.GetType());
            _environmentStateProvider.LinkCache.ResolveAllContexts(record.FormKey, registration.GetterType);
        }
    }

    public bool TryResolveTypedWithGenericFallback(FormKey formKey, Type? formType, out IMajorRecordGetter? record)
    {
        if (_environmentStateProvider == null || _environmentStateProvider.LinkCache == null)
        {
            record = null;
            return false;
        }

        if (formType != null)
        {
            var registration = LoquiRegistration.StaticRegister.GetRegister(formType);
            if (registration != null && _environmentStateProvider.LinkCache.TryResolve(formKey, registration.GetterType, out record))
            {
                return true;
            }
        }

        if (_environmentStateProvider.LinkCache.TryResolve(formKey, out record))
        {
            return true;
        }

        return false;
    }
}
