using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
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

    public void SaveSnapshots(ModKey[] modKeys)
    {
        if (_environmentStateProvider.LinkCache == null)
        {
            throw new Exception("Link Cache is null");
        }

        SerializationType serialization = SerializationType.JSON; // temporary until added to settings
        string extension = "";
        switch (serialization)
        {
            case SerializationType.JSON: extension = ".json"; break;
            case SerializationType.YAML: extension = ".yaml"; break;
        }

        var now = DateTime.Now;
        string dateStr = VM_Snapshot.ToLabelString(now);
        string dirPath = Path.Combine(_settingsProvider.Settings.SnapshotPath, dateStr);
        IOFunctions.CreateDirectoryIfNeeded(dirPath, IOFunctions.PathType.Directory);

        foreach (var targetModKey in modKeys)
        {
            var modSnapshot = TakeSnapShot(targetModKey, serialization, now);
            string filePath = Path.Combine(dirPath, targetModKey.Name + extension);
            JSONhandler<ModSnapshot>.SaveJSONFile(modSnapshot, filePath, out _, out _);
        }

        MessageBox.Show("Saved Snapshot " + dirPath);
    }

    public ModSnapshot TakeSnapShot(ModKey targetModKey, SerializationType serializationFormat, DateTime now)
    {
        var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(targetModKey);
        if (modListing == null)
        {
            MessageBox.Show("Could not find mod " + targetModKey.ToString() + " in current load order", "Error");
            return new();
        }

        var records = modListing?.Mod?.EnumerateMajorRecords();
        if (records == null)
        {
            throw new Exception("Records are null");
        }

        ModSnapshot modSnapshot = new();
        modSnapshot.CRModKey = targetModKey;
        modSnapshot.DateTaken = now;

        foreach (var record in records)
        {
            var formSnapShot = new FormSnapshot();
            formSnapShot.FormKey = record.FormKey;
            var contexts = _environmentStateProvider.LinkCache?.ResolveAllContexts(record.FormKey).ToList() ?? new();
            contexts.Reverse();
            formSnapShot.OverrideOrder = contexts.Select(x => x.ModKey).ToList();

            if (contexts == null)
            {
                throw new Exception("Contexts are null");
            }

            foreach (var context in contexts)
            {
                var contextSnapShot = new FormContextSnapshot();
                contextSnapShot.SourceModKey = context.ModKey;
                contextSnapShot.SerializationType = serializationFormat;
                var serialized = _serializer.SerializeRecord(context, serializationFormat); // serialize here
                if (formSnapShot.RecordType.IsNullOrWhitespace())
                {
                    formSnapShot.RecordType = serialized.Item1;
                }
                contextSnapShot.SerializationString = serialized.Item2;
                formSnapShot.ContextSnapshots.Add(contextSnapShot);
            }
            modSnapshot.Snapshots.Add(formSnapShot);
        }

        return modSnapshot;
    }
}
