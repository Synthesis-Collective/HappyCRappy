using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HappyCRappy;

public class SnapShotter
{
    private readonly SettingsProvider _settingsProvider;
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    public SnapShotter(SettingsProvider settingsProvider, IEnvironmentStateProvider environmentStateProvider)
    {
        _settingsProvider = settingsProvider;
        _environmentStateProvider = environmentStateProvider;
    }

    public void TakeSnapShot()
    {
        if (_environmentStateProvider.LinkCache == null)
        {
            throw new Exception("Link Cache is null");
        }

        List<FormSnapshot> snapShots = new();

        SerializationType serialization = SerializationType.JSON; // temporary until added to settings

        foreach (var targetModKey in _settingsProvider.Settings.TrackedModKeys)
        {
            var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(targetModKey);
            if (modListing != null)
            {
                MessageBox.Show("Could not find mod " + targetModKey.ToString() + " in current load order", "Error");
                continue;
            }

            var records = modListing?.Mod?.EnumerateMajorRecords();
            if (records == null)
            {
                throw new Exception("Records are null");
            }

            foreach (var record in records)
            {
                var formSnapShot = new FormSnapshot();
                formSnapShot.FormKey = record.FormKey;
                var contexts = _environmentStateProvider.LinkCache.ResolveAllContexts(record);
                formSnapShot.OverrideOrder = contexts.Select(x => x.ModKey).ToList();

                if (contexts == null)
                {
                    throw new Exception("Contexts are null");
                }

                foreach (var context in contexts)
                {
                    var contextSnapShot = new FormContextSnapshot();
                    contextSnapShot.SourceModKey = context.ModKey;
                    contextSnapShot.SerializationType = serialization;
                    contextSnapShot.SerializationString = ""; // serialize here
                    formSnapShot.ContextSnapshots.Add(contextSnapShot);
                }
            }
        }
    }
}
