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
    private readonly Serializer _serializer;
    public SnapShotter(SettingsProvider settingsProvider, IEnvironmentStateProvider environmentStateProvider, Serializer serializer)
    {
        _settingsProvider = settingsProvider;
        _environmentStateProvider = environmentStateProvider;
        _serializer = serializer;
    }

    public void TakeSnapShot()
    {
        if (_environmentStateProvider.LinkCache == null)
        {
            throw new Exception("Link Cache is null");
        }

        List<FormSnapshot> snapShots = new();

        SerializationType serialization = SerializationType.JSON; // temporary until added to settings
        string extension = "";
        switch(serialization)
        {
            case SerializationType.JSON: extension = ".json"; break;
            case SerializationType.YAML: extension = ".yaml"; break;
        }

        string dirPath = Path.Combine(_settingsProvider.Settings.SnapshotPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");
        IOFunctions.CreateDirectoryIfNeeded(dirPath, IOFunctions.PathType.Directory);

        foreach (var targetModKey in _settingsProvider.Settings.TrackedModKeys)
        {
            var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(targetModKey);
            if (modListing == null)
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
                var contexts = _environmentStateProvider.LinkCache.ResolveAllContexts(record.FormKey).ToList();
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
                    contextSnapShot.SerializationString = _serializer.SerializeRecord(context, serialization); // serialize here
                    formSnapShot.ContextSnapshots.Add(contextSnapShot);
                }
                snapShots.Add(formSnapShot);
            }
            string filePath = Path.Combine(dirPath, targetModKey.Name + extension);
            JSONhandler<List<FormSnapshot>>.SaveJSONFile(snapShots, filePath, out _, out _);
        }        
    }
}
