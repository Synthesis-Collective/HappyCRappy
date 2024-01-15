using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;
using Noggog;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows;

namespace HappyCRappy;

public class VM_LoadOrderMenu : VM
{
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    public VM_LoadOrderMenu(IEnvironmentStateProvider environmentStateProvider, VM_ModKeyWrapper.Factory loadOrderEntryFactory, VM_LoadOrderStash.Factory snapshotFactory, VM_SettingsMenu settingsVM)
    {
        _environmentStateProvider = environmentStateProvider;
        _snapshotFactory = snapshotFactory;
        SettingsVM = settingsVM;

        if (_environmentStateProvider.LoadOrder != null)
        {
            foreach (var entry in _environmentStateProvider.LoadOrder)
            {
                LoadOrder.Add(loadOrderEntryFactory(entry.Key));
            }
        }

        this.WhenAnyValue(x => x.SelectedStashDate).Subscribe(_ =>
        {
            if (_initialized)
            {
                RestoreLoadOrderStash();
            }
        }).DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedStash).Subscribe(_ =>
        {
            if (_initialized)
            {
                RefreshAvailability();
            }
        }).DisposeWith(this);

        SaveLoadOrderStashCommand = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                SaveLoadOrderStash();
            }
        );

        ApplyLoadOrderStashCommand = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                ApplyLoadOrderStash();
            }
        );

        this.WhenAnyValue(x => x.SettingsVM.SnapshotPath).Subscribe(_ => RefreshAvailableStashDates()).DisposeWith(this);

        _initialized = true;
    }

    public ObservableCollection<VM_ModKeyWrapper> LoadOrder { get; set; } = new();
    public ObservableCollection<VM_ModKeyWrapper> SelectedMods { get; set; } = new();
    public VM_LoadOrderStash? SelectedStash { get; set; }
    public ObservableCollection<string> AvailableStashDates { get; set; } = new();
    public string? SelectedStashDate { get; set; }
    private VM_LoadOrderStash.Factory _snapshotFactory;
    private bool _initialized = false;
    private VM_SettingsMenu SettingsVM;
    public RelayCommand SaveLoadOrderStashCommand { get; }
    public RelayCommand ApplyLoadOrderStashCommand { get; }
    public bool ToggleApplyLoadOrderStash { get; set; } = false;
    public LoadOrderStash? StashToApply { get; set; }

    public void RefreshAvailability()
    {
        foreach (var mod in LoadOrder)
        {
            mod.RefreshAvailability();
        }
    }

    public void Initialize()
    {
        SelectedStash = _snapshotFactory();
    }

    private void SaveLoadOrderStash()
    {
        if (SelectedStash == null)
        {
            MessageBox.Show("No load order stash is selected");
            return;
        }
        else if (!SelectedStash.ModChunks.Any() || !SelectedStash.ModChunks.First().Mods.Any())
        {
            MessageBox.Show("No plugins are currently being managed");
            return;
        }

        var now = DateTime.Now;
        string dateStr = VM_ModDisplay.ToLabelString(now);
        string dirPath = Path.Combine(SettingsVM.LoadOrderStashPath, dateStr);
        string filePath = Path.Combine(dirPath, "LoadOrderStash.json");
        IOFunctions.CreateDirectoryIfNeeded(dirPath, IOFunctions.PathType.Directory);

        var stash = SelectedStash.DumpToModel();
        stash.DateTaken = now;

        JSONhandler<LoadOrderStash>.SaveJSONFile(stash, filePath, out bool success, out string exceptionStr);
        if(success)
        {
            MessageBox.Show("Stashed managed plugins to " + filePath);
        }
        else
        {
            MessageBox.Show(exceptionStr);
        }
    }

    private void RestoreLoadOrderStash()
    {
        if (SelectedStashDate == null || SelectedStashDate.IsNullOrEmpty())
        {
            MessageBox.Show("No load order stash was selected");
            return;
        }

        var stashPath = Path.Combine(SettingsVM.LoadOrderStashPath, SelectedStashDate, "LoadOrderStash.json");
        if (!File.Exists(stashPath))
        {
            MessageBox.Show("Error: could not find file at " + stashPath);
            return;
        }

        var loadedStash = JSONhandler<LoadOrderStash>.LoadJSONFile(stashPath, out bool success, out string exceptionStr);
        if (!success || loadedStash == null)
        {
            MessageBox.Show("Error loading stash file: " + exceptionStr);
            return;
        }

        SelectedStash = _snapshotFactory();
        SelectedStash.CopyInFromModel(loadedStash);
    }

    private void ApplyLoadOrderStash()
    {
        if (SelectedStash == null)
        {
            MessageBox.Show("No load order stash was selected");
            return;
        }

        if (SelectedStash.ModChunks == null || !SelectedStash.ModChunks.Where(x => x.Mods.Any()).Any())
        {
            MessageBox.Show("The selected load order stash has no entries");
            return;
        }

        ToggleApplyLoadOrderStash = true;
        StashToApply = SelectedStash.DumpToModel();
        MessageBox.Show("The currently display load order adjustments will be applied when this application is closed");
    }

    private void RefreshAvailableStashDates()
    {
        if (!Directory.Exists(SettingsVM.LoadOrderStashPath))
        {
            return;
        }

        List<string> currentDirNames = new();
        foreach (var directory in Directory.GetDirectories(SettingsVM.LoadOrderStashPath))
        {
            string dirName = Path.GetFileName(directory);
            currentDirNames.Add(dirName);
            if (!AvailableStashDates.Contains(dirName))
            {
                AvailableStashDates.Add(dirName);
            }
        }

        AvailableStashDates.RemoveWhere(x => !currentDirNames.Contains(x));
    }
}

public class VM_ModKeyWrapper : VM
{
    public delegate VM_ModKeyWrapper Factory(ModKey modKey);
    public VM_ModKeyWrapper(ModKey modKey, Func<VM_LoadOrderMenu> parentMenu)
    {
        ModKey = modKey;
        _parentMenu = parentMenu;
    }

    public ModKey ModKey { get; }
    public bool IsManaged { get; set; }
    public SolidColorBrush BorderColor { get; set; } = new(Colors.Green);
    public bool IsSelected { get; set; }
    private readonly Func<VM_LoadOrderMenu> _parentMenu;

    public void RefreshAvailability()
    {
        if (_parentMenu().SelectedStash != null && 
            _parentMenu().SelectedStash.ModChunks != null &&
            _parentMenu().SelectedStash.ModChunks.Where(x => x.Mods.Select(y => y.ModKey).Contains(ModKey)).Any())
        {
            IsManaged = true;
            BorderColor = new(Colors.Gray);
        }
        else
        {
            IsManaged = false;
            BorderColor = new(Colors.Green);
        }
    }
}
