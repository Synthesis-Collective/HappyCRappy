using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class HappyCrappySettings
{
    public string DataFolderPath { get; set; } = string.Empty;
    public SkyrimRelease GameType { get; set; } = SkyrimRelease.SkyrimSE;
    public List<ModKey> TrackedModKeys { get; set; } = new();
    public string SnapshotPath { get; set; } = string.Empty;
    public SerializationType SerializationSaveFormat { get; set; } = SerializationType.JSON;
    public SerializationType SerializationViewDisplay { get; set; } = SerializationType.JSON;
    public string LoadOrderStashPath { get; set; } = string.Empty;
    public bool ShowOnlyConflicts { get; set; } = true;
    public bool ShowPotentialConflicts { get; set; } = true;
    public bool HandleRemappedFormTypes { get; set; } = true;
    public bool WarmUpLinkCacheOnStartup { get; set; } = true;
    public bool UseDeepCacheing { get; set; } = true;
}
