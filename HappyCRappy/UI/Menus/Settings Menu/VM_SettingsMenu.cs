using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Noggog;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;

namespace HappyCRappy;

public class VM_SettingsMenu : VM
{
    public VM_SettingsMenu(IEnvironmentStateProvider environmentStateProvider, SettingsProvider settingsProvider)
    {
        EnvironmentStateProvider = environmentStateProvider;
        _settingsProvider = settingsProvider;

        SnapshotPath = Path.Combine(settingsProvider.GetExePath(), "Snapshots");

        SetSnapshotPath = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                if (IOFunctions.SelectFolder("", out string selectedPath))
                {
                    SnapshotPath = selectedPath;
                }
            }
        );

        LoadOrderStashPath = Path.Combine(settingsProvider.GetExePath(), "Load Order Stashes");

        SetLoadOrderStashPath = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                if (IOFunctions.SelectFolder("", out string selectedPath))
                {
                    LoadOrderStashPath = selectedPath;
                }
            }
        );
    }

    public IEnvironmentStateProvider EnvironmentStateProvider { get; }
    private readonly SettingsProvider _settingsProvider;
    public ObservableCollection<ModKey> TrackedModKeys { get; set; } = new();
    public string SnapshotPath { get; set; } = string.Empty;
    public SerializationType SerializationFormat { get; set; } = SerializationType.JSON;
    public RelayCommand SetSnapshotPath { get; }
    public string LoadOrderStashPath { get; set; } = string.Empty;
    public RelayCommand SetLoadOrderStashPath { get; }
    public bool HandleRemappedFormTypes { get; set; } = true;
    public bool WarmUpLinkCacheOnStartup { get; set; } = true;
    public bool UseDeepCacheing { get; set; } = true;
    public void ReadFromModel(HappyCrappySettings model)
    {
        TrackedModKeys = new(model.TrackedModKeys);
        if (!model.SnapshotPath.IsNullOrWhitespace() && Directory.Exists(model.SnapshotPath))
        {
            SnapshotPath = model.SnapshotPath;
        }
        else
        {
            SnapshotPath = Path.Combine(_settingsProvider.GetExePath(), "Snapshots");
        }
        SerializationFormat = model.SerializationSaveFormat;

        if (!model.LoadOrderStashPath.IsNullOrWhitespace() && Directory.Exists(model.LoadOrderStashPath))
        {
            LoadOrderStashPath = model.LoadOrderStashPath;
        }
        else
        {
            LoadOrderStashPath = Path.Combine(_settingsProvider.GetExePath(), "Load Order Stashes");
        }

        if (EnvironmentStateProvider is StandaloneEnvironmentStateProvider standaloneEnvironment)
        {
            standaloneEnvironment.GameType = model.GameType;
            standaloneEnvironment.DataFolderPath = model.DataFolderPath;
        }

        UseDeepCacheing = model.UseDeepCacheing;
        HandleRemappedFormTypes = model.HandleRemappedFormTypes;
        WarmUpLinkCacheOnStartup = model.WarmUpLinkCacheOnStartup;
    }

    public HappyCrappySettings DumpToModel()
    {
        HappyCrappySettings model = new()
        {
            GameType = EnvironmentStateProvider.GameType,
            DataFolderPath = EnvironmentStateProvider.DataFolderPath,
            TrackedModKeys = TrackedModKeys.ToList(),
            SnapshotPath = SnapshotPath,
            SerializationSaveFormat = SerializationFormat,
            UseDeepCacheing = UseDeepCacheing,
            HandleRemappedFormTypes = HandleRemappedFormTypes,
            WarmUpLinkCacheOnStartup = WarmUpLinkCacheOnStartup
        };
        return model;
    }
}
