using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public interface IEnvironmentStateProvider
{
    ILoadOrderGetter<IModListingGetter<ISkyrimModGetter>>? LoadOrder { get; }
    ILinkCache<ISkyrimMod, ISkyrimModGetter>? LinkCache { get; }
    DirectoryPath ExtraSettingsDataPath { get; }
    DirectoryPath InternalDataPath { get; }
    DirectoryPath DataFolderPath { get; set; }
    public GameRelease GameType { get; }
    // Additional properties (for logging only)
    public string? CreationClubListingsFilePath { get; }
    public string? LoadOrderFilePath { get; }
}
