using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace HappyCRappy;

public class VM_LoadOrderBlock : VM
{
    public delegate VM_LoadOrderBlock Factory();
    public VM_LoadOrderBlock(VM_LoadOrderMenu parentMenu)
    {
        _parentMenu = parentMenu;

        Mods.ToObservableChangeSet().Subscribe(_ => _parentMenu.RefreshAvailability()).DisposeWith(this);
    }
    private readonly VM_LoadOrderMenu _parentMenu;
    public ObservableCollection<ModKey> Mods { get; set; } = new();
    public ModKey? PlaceAfter { get; set; }
    public ModKey? PlaceBefore { get; set; }

    public void CopyInFromModel(LoadOrderBlock model)
    {
        Mods = new(model.Mods);
        PlaceAfter = model.PlaceAfter;
        PlaceBefore = model.PlaceBefore;
    }

    public LoadOrderBlock DumpToModel()
    {
        return new()
        {
            Mods = Mods.ToList(),
            PlaceAfter = PlaceAfter,
            PlaceBefore = PlaceBefore
        };
    }
}
