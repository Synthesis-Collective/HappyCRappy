using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Noggog;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Controls;
using System.Windows;

namespace HappyCRappy;

public class VM_LoadOrderBlock : VM, IDropTarget
{
    public delegate VM_LoadOrderBlock Factory(VM_LoadOrderSnapshot parentSnapshot);
    public VM_LoadOrderBlock(VM_LoadOrderSnapshot parentSnapshot, VM_LoadOrderMenu parentMenu, VM_ModKeyWrapper.Factory modWrapperFactory)
    {
        _parentSnapshot = parentSnapshot;
        _parentMenu = parentMenu;
        _modWrapperFactory = modWrapperFactory;

        Mods.ToObservableChangeSet().Subscribe(_ => {
            _parentMenu.RefreshAvailability();
            RefreshPrePostMods();
            }).DisposeWith(this);

        RemoveSelectedMod = new RelayCommand(
            canExecute: _ => true,
            execute: x =>
            {
                Mods.Remove((VM_ModKeyWrapper)x);
                DeleteIfNecessary();
            }
        );
    }
    private readonly VM_LoadOrderMenu _parentMenu;
    private readonly VM_LoadOrderSnapshot _parentSnapshot;
    private readonly VM_ModKeyWrapper.Factory _modWrapperFactory;
    public ObservableCollection<VM_ModKeyWrapper> Mods { get; set; } = new();
    public VM_ModKeyWrapper? PlaceAfter { get; set; }
    public ObservableCollection<VM_ModKeyWrapper> AvailablePriorMods { get; set; } = new();
    public VM_ModKeyWrapper? PlaceBefore { get; set; }
    public ObservableCollection<VM_ModKeyWrapper> AvailableSubsequentMods { get; set; } = new();
    public RelayCommand RemoveSelectedMod { get; }

    public void CopyInFromModel(LoadOrderBlock model)
    {
        Mods = new(model.Mods.Select(x => _modWrapperFactory(x)));
        PlaceAfter = model.PlaceAfter != null ? _modWrapperFactory(model.PlaceAfter.Value) : null;
        PlaceBefore = model.PlaceBefore != null ? _modWrapperFactory(model.PlaceBefore.Value) : null;
    }

    public LoadOrderBlock DumpToModel()
    {
        return new()
        {
            Mods = Mods.Select(x => x.ModKey).ToList(),
            PlaceAfter = PlaceAfter?.ModKey ?? new ModKey(),
            PlaceBefore = PlaceBefore?.ModKey ?? new ModKey()
        };
    }

    public void RefreshPrePostMods()
    {
        List<int> indices = new();
        foreach (var mod in Mods)
        {
            var loadOrderMatch = _parentMenu.LoadOrder.Where(x => x.ModKey.Equals(mod.ModKey)).FirstOrDefault();
            if (loadOrderMatch != null)
            {
                indices.Add(_parentMenu.LoadOrder.IndexOf(loadOrderMatch));
            }
        }

        if(indices.Any())
        {
            var ordered = indices.OrderBy(x => x).ToArray();
            var first = ordered.First();
            var last = ordered.Last();

            var availableFirst = new List<VM_ModKeyWrapper>();
            var availableLast = new List<VM_ModKeyWrapper>();
            if (first > 0)
            {
                for (int i = 0; i < first; i++)
                {
                    availableFirst.Add(_parentMenu.LoadOrder[i]);
                }
            }

            if (last < _parentMenu.LoadOrder.Count - 1)
            {
                for (int i = last + 1; i < _parentMenu.LoadOrder.Count; i++)
                {
                    availableLast.Add(_parentMenu.LoadOrder[i]);
                }
            }

            for (int i = 0; i < AvailablePriorMods.Count; i++)
            {
                if (!availableFirst.Where(x => x.ModKey.Equals(AvailablePriorMods[i].ModKey)).Any())
                {
                    AvailablePriorMods.RemoveAt(i);
                    i--;
                }
            }

            foreach (var firstMod in availableFirst)
            {
                if (!AvailablePriorMods.Where(x => x.ModKey.Equals(firstMod.ModKey)).Any())
                {
                    AvailablePriorMods.Add(firstMod);
                }
            }

            for (int i = 0; i < AvailableSubsequentMods.Count; i++)
            {
                if (!availableLast.Where(x => x.ModKey.Equals(AvailableSubsequentMods[i].ModKey)).Any())
                {
                    AvailableSubsequentMods.RemoveAt(i);
                    i--;
                }
            }

            foreach (var lastMod in availableLast)
            {
                if (!AvailableSubsequentMods.Where(x => x.ModKey.Equals(lastMod.ModKey)).Any())
                {
                    AvailableSubsequentMods.Add(lastMod);
                }
            }

            if (AvailablePriorMods.Any())
            {
                PlaceAfter = AvailablePriorMods.Last();
            }

            if (AvailableSubsequentMods.Any())
            {
                PlaceBefore = AvailableSubsequentMods.First();
            }
        }
    }

    public void DeleteIfNecessary()
    {
        if(!Mods.Any() && _parentSnapshot.ModChunks.Count > 1)
        {
            _parentSnapshot.ModChunks.Remove(this);
        }
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is VM_ModKeyWrapper)
        {
            var draggedMod = (VM_ModKeyWrapper)dropInfo.Data;
            if (draggedMod != null)
            {
                if(draggedMod.IsManaged)
                {
                    dropInfo.Effects = DragDropEffects.None;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is VM_ModKeyWrapper)
        {
            var draggedMod = (VM_ModKeyWrapper)dropInfo.Data;
            if (dropInfo.TargetCollection != null && !draggedMod.IsManaged)
            {
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<VM_ModKeyWrapper>;
                if (targetCollection != null)
                {
                    targetCollection.Add(draggedMod);
                }
            }
        }
    }
}
