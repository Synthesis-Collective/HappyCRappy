using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Noggog;

namespace HappyCRappy;

public class VM_SnapshotMenu : VM
{
    private readonly SettingsProvider _settingsProvider;
    public VM_SnapshotMenu(SettingsProvider settingsProvider, SnapShotter snapShotter) 
    {
        _settingsProvider = settingsProvider;

        TakeSnapShot = new RelayCommand(
                canExecute: _ => true,
                execute: _ =>
                {
                    snapShotter.TakeSnapShot();
                }
            );

        this.WhenAnyValue(x => x.SelectedSnapshotDateStr).Subscribe(dateStr =>
        {
            LoadSnapshot(dateStr);
        }).DisposeWith(this);

        this.WhenAnyValue(x => x.DisplayAsJson, x => x.DisplayAsYaml).Subscribe(
            _ =>
            {
                if (DisplayAsJson)
                {
                    SerializationType = SerializationType.JSON;
                }
                else if (DisplayAsYaml)
                {
                    SerializationType = SerializationType.YAML;
                }
            }).DisposeWith(this);
    }

    public RelayCommand TakeSnapShot { get; }
    public SerializationType SerializationType { get; set; } = SerializationType.JSON;
    public bool DisplayAsJson { get; set; }
    public bool DisplayAsYaml { get; set; }
    public string SelectedSnapshotDateStr { get; set; } = string.Empty;
    public ObservableCollection<string> AvailableSnapshotDates { get; set; } = new();
    public VM_Snapshot? DisplayedSnapshot { get; set; }

    public void ReadFromModel(HappyCrappySettings settings)
    {
        SerializationType = settings.SerializationViewDisplay;
        switch(SerializationType)
        {
            case SerializationType.JSON: DisplayAsJson = true; DisplayAsYaml = false; break;
            case SerializationType.YAML: DisplayAsJson = false; DisplayAsYaml = true; break;
        }
    }

    public void SaveSettings()
    {
        _settingsProvider.Settings.SerializationViewDisplay = SerializationType;
    }
    private void LoadSnapshot(string dateStr)
    {
        if (!Directory.Exists(_settingsProvider.Settings.SnapshotPath))
        {
            return;
        }
        foreach (var snapShotPath in Directory.GetFiles(_settingsProvider.Settings.SnapshotPath))
        {
            ModSnapshot? snapshot = JSONhandler<ModSnapshot>.LoadJSONFile(snapShotPath, out bool loaded, out string exceptionStr);
            if (loaded && snapshot != null && VM_Snapshot.ToLabelString(snapshot.DateTaken) == dateStr)
            {
                DisplayedSnapshot = new(snapshot);
                break;
            }
        }
    }

    private void RefreshAvailableSnapshots()
    {

    }
}
