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

namespace HappyCRappy;

public class VM_LoadOrderMenu : VM
{
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    public VM_LoadOrderMenu(IEnvironmentStateProvider environmentStateProvider, VM_ModKeyWrapper.Factory loadOrderEntryFactory, VM_LoadOrderSnapshot.Factory snapshotFactory)
    {
        _environmentStateProvider = environmentStateProvider;
        _snapshotFactory = snapshotFactory;

        if (_environmentStateProvider.LoadOrder != null)
        {
            foreach (var entry in _environmentStateProvider.LoadOrder)
            {
                LoadOrder.Add(loadOrderEntryFactory(entry.Key));
            }
        }

        this.WhenAnyValue(x => x.SelectedSnapshot).Subscribe(_ =>
        {
            RefreshAvailability();
        }).DisposeWith(this);

    }

    public ObservableCollection<VM_ModKeyWrapper> LoadOrder { get; set; } = new();
    public ObservableCollection<VM_ModKeyWrapper> SelectedMods { get; set; } = new();
    public VM_LoadOrderSnapshot? SelectedSnapshot { get; set; }
    private VM_LoadOrderSnapshot.Factory _snapshotFactory;

    public void RefreshAvailability()
    {
        foreach (var mod in LoadOrder)
        {
            mod.RefreshAvailability();
        }
    }

    public void Initialize()
    {
        SelectedSnapshot = _snapshotFactory();
    }
}

public class VM_ModKeyWrapper : VM
{
    public delegate VM_ModKeyWrapper Factory(ModKey modKey);
    public VM_ModKeyWrapper(ModKey modKey)
    {
        ModKey = modKey;
    }

    public ModKey ModKey { get; }
    public bool IsManaged { get; set; }
    public SolidColorBrush BorderColor { get; set; } = new(Colors.Green);
    public bool IsSelected { get; set; }

    public void RefreshAvailability()
    {

    }
}
