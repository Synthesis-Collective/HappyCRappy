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
}
