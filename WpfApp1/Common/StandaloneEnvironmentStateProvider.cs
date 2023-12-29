using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda;

namespace HappyCRappy;

public class StandaloneEnvironmentStateProvider : VM, IEnvironmentStateProvider
{
    // "Core" state properties and fields
    private IGameEnvironment<ISkyrimMod, ISkyrimModGetter>? _environment;
    public ILoadOrderGetter<IModListingGetter<ISkyrimModGetter>>? LoadOrder => _environment?.LoadOrder;
    public ILinkCache<ISkyrimMod, ISkyrimModGetter>? LinkCache => _environment?.LinkCache;
    [Reactive] public GameRelease GameType { get; set; } = GameRelease.SkyrimSE;
    public DirectoryPath ExtraSettingsDataPath { get; set; }
    public DirectoryPath InternalDataPath { get; set; }
    [Reactive] public DirectoryPath DataFolderPath { get; set; }
    public string? CreationClubListingsFilePath { get; set; }
    public string? LoadOrderFilePath { get; set; }
    public bool EnvironmentCreated { get; set; }

    public StandaloneEnvironmentStateProvider()
    {
        UpdateEnvironment();

        this.WhenAnyValue(
                x => x.GameType,
                x => x.DataFolderPath)
            .Subscribe(_ => UpdateEnvironment())
            .DisposeWith(this);
    }

    public void UpdateEnvironment()
    {
        var builder = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameType);
        if (!DataFolderPath.ToString().IsNullOrWhitespace())
        {
            builder = builder.WithTargetDataFolder(DataFolderPath);
        }

        var built = false;

        try
        {
            _environment = builder
                .TransformModListings(x =>
                    x.OnlyEnabledAndExisting())
                .Build();
            built = true;

            CreationClubListingsFilePath = _environment.CreationClubListingsFilePath ?? string.Empty;
            LoadOrderFilePath = _environment.LoadOrderFilePath;
            DataFolderPath = _environment.DataFolderPath; // If a custom data folder path was provided it will not change. If no custom data folder path was provided, this will set it to the default path.
            EnvironmentCreated = true;
        }
        catch (Exception ex)
        {
            string exStr = ExceptionLogger.GetExceptionStack(ex);
            EnvironmentCreated = false;
        }
    }
}
