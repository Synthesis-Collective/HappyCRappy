using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Plugins;

namespace HappyCRappy;

public class VM_SnapshotMenu : VM
{
    private readonly SettingsProvider _settingsProvider;
    private readonly SnapShotter _snapShotter;
    public VM_SnapshotMenu(SettingsProvider settingsProvider, VM_SettingsMenu settingsVM, SnapShotter snapShotter) 
    {
        _settingsProvider = settingsProvider;
        SettingsVM = settingsVM;
        _snapShotter = snapShotter;

        TakeSnapShot = new RelayCommand(
                canExecute: _ => true,
                execute: _ =>
                {
                    _snapShotter.SaveSnapshots(settingsVM.TrackedModKeys.ToArray());
                    RefreshAvailableSnapshots();
                }
            );

        this.WhenAnyValue(x => x.SelectedSnapshotDateStr, x => x.SelectedSnapshotMod).Subscribe(selection =>
        {
            LoadSnapshot(selection.Item1, selection.Item2);
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

        this.WhenAnyValue(x => x.SettingsVM.SnapshotPath).Subscribe(_ => RefreshAvailableSnapshots()).DisposeWith(this);
    }

    public RelayCommand TakeSnapShot { get; }
    public SerializationType SerializationType { get; set; } = SerializationType.JSON;
    public bool DisplayAsJson { get; set; }
    public bool DisplayAsYaml { get; set; }
    public ModKey? SelectedSnapshotMod { get; set; }
    public string? SelectedSnapshotDateStr { get; set; }
    public ObservableCollection<string> AvailableSnapshotDates { get; set; } = new();
    public VM_Snapshot? DisplayedSnapshot { get; set; }
    public VM_SettingsMenu SettingsVM { get; }

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
    private void LoadSnapshot(string? dateStr, ModKey? crModKey)
    {
        if (!Directory.Exists(_settingsProvider.Settings.SnapshotPath) || dateStr == null || dateStr.IsNullOrWhitespace() || crModKey == null)
        {
            return;
        }
        foreach (var snapShotPath in Directory.GetFiles(_settingsProvider.Settings.SnapshotPath))
        {
            ModSnapshot? snapshot = JSONhandler<ModSnapshot>.LoadJSONFile(snapShotPath, out bool loaded, out string exceptionStr);
            if (loaded && snapshot != null && VM_Snapshot.ToLabelString(snapshot.DateTaken) == dateStr && snapshot.CRModKey == SelectedSnapshotMod)
            {
                var currentSnapshot = _snapShotter.TakeSnapShot(crModKey.Value, SerializationType, DateTime.Now);
                DisplayedSnapshot = new(snapshot, currentSnapshot);
                break;
            }
        }
    }

    private void RefreshAvailableSnapshots()
    {
        if (!Directory.Exists(SettingsVM.SnapshotPath))
        {
            return;
        }

        List<string> currentDirNames = new();
        foreach (var directory in Directory.GetDirectories(SettingsVM.SnapshotPath))
        {
            string dirName = Path.GetFileName(directory);
            currentDirNames.Add(dirName);
            if (!AvailableSnapshotDates.Contains(dirName))
            {
                AvailableSnapshotDates.Add(dirName);
            }
        }

        AvailableSnapshotDates.RemoveWhere(x => !currentDirNames.Contains(x));
    }
}
