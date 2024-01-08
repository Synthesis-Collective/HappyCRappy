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
using System.Windows.Media;

namespace HappyCRappy;

public class StandaloneEnvironmentStateProvider : VM, IEnvironmentStateProvider
{
    // "Core" state properties and fields
    private IGameEnvironment<ISkyrimMod, ISkyrimModGetter>? _environment;
    public ILoadOrderGetter<IModListingGetter<ISkyrimModGetter>>? LoadOrder => _environment?.LoadOrder;
    public ILinkCache<ISkyrimMod, ISkyrimModGetter>? LinkCache => _environment?.LinkCache;
    [Reactive] public SkyrimRelease GameType { get; set; } = SkyrimRelease.SkyrimSE;
    public DirectoryPath ExtraSettingsDataPath { get; set; }
    public DirectoryPath InternalDataPath { get; set; }
    [Reactive] public DirectoryPath DataFolderPath { get; set; }
    public string? CreationClubListingsFilePath { get; set; }
    public string? LoadOrderFilePath { get; set; }
    public bool EnvironmentCreated { get; set; }
    public RelayCommand SelectGameDataFolder { get; }
    public RelayCommand ClearGameDataFolder { get; }
    public SolidColorBrush EnvironmentColor { get; set; } = ColorEnvironmentError;
    public static SolidColorBrush ColorEnvironmentOK = new SolidColorBrush(Colors.Green);
    public static SolidColorBrush ColorEnvironmentError = new SolidColorBrush(Colors.Red);
    public string StatusText { get; set; } = string.Empty;

    public StandaloneEnvironmentStateProvider()
    {
        UpdateEnvironment();

        this.WhenAnyValue(x => x.GameType)
            .Subscribe(_ => { 
                DataFolderPath = string.Empty;
                UpdateEnvironment();
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.DataFolderPath)
            .Subscribe(_ => UpdateEnvironment())
            .DisposeWith(this);

        SelectGameDataFolder = new RelayCommand(
                canExecute: _ => true,
                execute: _ =>
                {
                    if (IOFunctions.SelectFolder("", out string selectedPath))
                    {
                        DataFolderPath = selectedPath;
                    }
                }
            );

        ClearGameDataFolder = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DataFolderPath = string.Empty;
            }
        );
    }

    public void UpdateEnvironment()
    {
        var builder = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameType.ToGameRelease());
        if (!DataFolderPath.ToString().IsNullOrWhitespace())
        {
            builder = builder.WithTargetDataFolder(DataFolderPath);
        }

        try
        {
            _environment = builder
                .TransformModListings(x =>
                    x.OnlyEnabledAndExisting())
                .Build();

            CreationClubListingsFilePath = _environment.CreationClubListingsFilePath ?? string.Empty;
            LoadOrderFilePath = _environment.LoadOrderFilePath;
            DataFolderPath = _environment.DataFolderPath; // If a custom data folder path was provided it will not change. If no custom data folder path was provided, this will set it to the default path.
            EnvironmentCreated = true;
            EnvironmentColor = ColorEnvironmentOK;
            StatusText = "Environment is valid";
        }
        catch (Exception ex)
        {
            string exStr = ExceptionLogger.GetExceptionStack(ex);
            EnvironmentCreated = false;
            EnvironmentColor = ColorEnvironmentError;
            StatusText = "Environment is invalid: " + ex.Message;
        }
    }

    public void SetGameType(SkyrimRelease gameType)
    {
        GameType = gameType;
    }

    public void SetDataFolder(string dataFolder)
    {
        DataFolderPath = dataFolder;
    }
}
