using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;

namespace HappyCRappy;

public class VM_LoadOrderSnapshot : VM
{
    public delegate VM_LoadOrderSnapshot Factory();
    public VM_LoadOrderSnapshot(VM_LoadOrderMenu parentMenu)
    {
        _parentMenu = parentMenu;

        this.ModChunks.ToObservableChangeSet().Subscribe(_ => _parentMenu.RefreshAvailability()).DisposeWith(this);
    }
    private readonly VM_LoadOrderMenu _parentMenu;
    public ObservableCollection<LoadOrderBlock> ModChunks { get; set; } = new();
    public DateTime DateTaken { get; set; }
    public string Version { get; set; } = VM_MainWindow._programVersion;

    public void CopyInFromModel(LoadOrderSnapshot model)
    {
        ModChunks = new(model.ModChunks);
        DateTaken = model.DateTaken;
        Version = model.Version;
    }

    public LoadOrderSnapshot DumpToModel()
    {
        return new()
        {
            ModChunks = ModChunks.ToList(),
            DateTaken = DateTaken,
            Version = Version
        };
    }
}
