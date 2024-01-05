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
    private readonly PotentialConflictFinder _conflictFinder;
    private readonly VM_ModDisplay.Factory _snapshotVMFactory;
    public VM_SnapshotMenu(SettingsProvider settingsProvider, VM_SettingsMenu settingsVM, SnapShotter snapShotter, PotentialConflictFinder conflictFinder, VM_ModDisplay.Factory snapshotVMFactory) 
    {
        _settingsProvider = settingsProvider;
        SettingsVM = settingsVM;
        _snapShotter = snapShotter;
        _conflictFinder = conflictFinder;
        _snapshotVMFactory = snapshotVMFactory;

        TakeSnapShot = new RelayCommand(
                canExecute: _ => true,
                execute: _ =>
                {
                    _snapShotter.SaveSnapshots(settingsVM.TrackedModKeys.ToArray(), SerializationType);
                    RefreshAvailableSnapshotDates();
                }
            );

        this.WhenAnyValue(x => x.SelectedSnapshotDateStr)
            .Subscribe(_ => RefreshAvailableSnapshotMods())
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedSnapshotMod)
            .Subscribe(_ => LoadSnapshot())
            .DisposeWith(this);

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

        this.WhenAnyValue(x => x.SettingsVM.SnapshotPath).Subscribe(_ => RefreshAvailableSnapshotDates()).DisposeWith(this);
    }

    public RelayCommand TakeSnapShot { get; }
    public SerializationType SerializationType { get; set; } = SerializationType.JSON;
    public bool DisplayAsJson { get; set; }
    public bool DisplayAsYaml { get; set; }
    public bool ShowOnlyConflicts { get; set; } = true;
    public bool ShowPotentialConflicts { get; set; } = true;
    private List<ModSnapshot> _loadedSnapshots { get; set; } = new();
    public ModKey? SelectedSnapshotMod { get; set; }
    public string? SelectedSnapshotDateStr { get; set; }
    public ObservableCollection<string> AvailableSnapshotDates { get; set; } = new();
    public ObservableCollection<ModKey> AvailableSnapshotMods { get; set; } = new();
    public VM_ModDisplay? DisplayedSnapshot { get; set; }
    public VM_SettingsMenu SettingsVM { get; }

    public void ReadFromModel(HappyCrappySettings settings)
    {
        SerializationType = settings.SerializationViewDisplay;
        switch(SerializationType)
        {
            case SerializationType.JSON: DisplayAsJson = true; DisplayAsYaml = false; break;
            case SerializationType.YAML: DisplayAsJson = false; DisplayAsYaml = true; break;
        }
        ShowOnlyConflicts = settings.ShowOnlyConflicts;
        ShowPotentialConflicts = settings.ShowPotentialConflicts;
    }

    public void SaveSettings()
    {
        _settingsProvider.Settings.SerializationViewDisplay = SerializationType;
        _settingsProvider.Settings.ShowOnlyConflicts = ShowOnlyConflicts;
        _settingsProvider.Settings.ShowPotentialConflicts = ShowPotentialConflicts;
    }
    private void LoadSnapshot()
    {
        if (!Directory.Exists(_settingsProvider.Settings.SnapshotPath) || SelectedSnapshotMod == null)
        {
            return;
        }
        ModSnapshot? snapshot = _loadedSnapshots.Where(x => x.CRModKey.Equals(SelectedSnapshotMod)).FirstOrDefault();
        if (snapshot != null)
        {
            var currentSnapshot = _snapShotter.TakeSnapShot(SelectedSnapshotMod.Value, SerializationType, DateTime.Now);

            var potentialConflicts = new Dictionary<string, List<PotentialConflictFinder.PotentialConflictRecord>>();
            if(ShowPotentialConflicts)
            {
                potentialConflicts = _conflictFinder.FindConflicts(SelectedSnapshotMod.Value, GetAllRootFormKeys(new List<ModSnapshot>() { currentSnapshot, snapshot}), SerializationType);
            }

            DisplayedSnapshot = _snapshotVMFactory(snapshot, currentSnapshot, potentialConflicts);
        }
    }

    private void RefreshAvailableSnapshotDates()
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
        AvailableSnapshotDates.Insert(0, "");
    }

    private void RefreshAvailableSnapshotMods()
    {
        if (SelectedSnapshotDateStr == null || SelectedSnapshotDateStr.IsNullOrEmpty())
        {
            return;
        }
        var selectedDir = Path.Combine(SettingsVM.SnapshotPath, SelectedSnapshotDateStr);
        if (!Directory.Exists(selectedDir))
        {
            return;
        }

        AvailableSnapshotMods.Clear();
        _loadedSnapshots.Clear();
        foreach (var entry in Directory.GetFiles(selectedDir))
        {
            var snapshot = JSONhandler<ModSnapshot>.LoadJSONFile(entry, out bool success, out _);
            if (success && snapshot != null)
            {
                AvailableSnapshotMods.Add(snapshot.CRModKey);
                _loadedSnapshots.Add(snapshot);
            }
        }
    }

    private List<FormKey> GetAllRootFormKeys(IEnumerable<ModSnapshot> snapshots)
    {
        List<FormKey> formKeys = new();
        foreach (var snapshot in snapshots)
        {
            foreach (var byType in snapshot.SnapshotsByType)
            {
                if (!formKeys.Contains(byType.FormKey))
                {
                    formKeys.Add(byType.FormKey);
                }
            }
        }
        return formKeys;
    }
}
