using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Newtonsoft.Json.Bson;

namespace HappyCRappy;

public class VM_LoadOrderSnapshot : VM
{
    public delegate VM_LoadOrderSnapshot Factory();
    public VM_LoadOrderSnapshot(VM_LoadOrderMenu parentMenu, VM_LoadOrderBlock.Factory blockFactory, VM_ModKeyWrapper.Factory modWrapperFactory)
    {
        _parentMenu = parentMenu;
        _blockFactory = blockFactory;
        _modWrapperFactory = modWrapperFactory;

        ModChunks.Add(blockFactory(this));

        this.ModChunks.ToObservableChangeSet().Subscribe(_ => _parentMenu.RefreshAvailability()).DisposeWith(this);

        AddBlock = new RelayCommand(
            canExecute: _ => true,
            execute: x =>
            {
                ModChunks.Add(blockFactory(this));
            }
        );
    }
    private readonly VM_LoadOrderMenu _parentMenu;
    private readonly VM_LoadOrderBlock.Factory _blockFactory;
    private readonly VM_ModKeyWrapper.Factory _modWrapperFactory;
    public ObservableCollection<VM_LoadOrderBlock> ModChunks { get; set; } = new();
    public DateTime DateTaken { get; set; }
    public string Version { get; set; } = VM_MainWindow._programVersion;
    public RelayCommand AddBlock { get; }

    public void CopyInFromModel(LoadOrderSnapshot model)
    {
        ModChunks.Clear();
        foreach (var chunk in model.ModChunks)
        {
            var chunkVM = _blockFactory(this);
            chunkVM.Mods.AddRange(chunk.Mods.Select(x => _modWrapperFactory(x)));
            ModChunks.Add(chunkVM);
        }
        DateTaken = model.DateTaken;
        Version = model.Version;
    }

    public LoadOrderSnapshot DumpToModel()
    {
        return new()
        {
            ModChunks = ModChunks.Select(x => x.DumpToModel()).ToList(),
            DateTaken = DateTaken,
            Version = Version
        };
    }
}
