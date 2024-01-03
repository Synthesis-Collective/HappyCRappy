using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace HappyCRappy;

public class VM_FormSnapshot : VM, ISnapshotDisplayNode
{
    public delegate VM_FormSnapshot Factory(FormSnapshot selectedSnapshot, FormSnapshot currentSnapshot);
    public VM_FormSnapshot(FormSnapshot selectedSnapshot, FormSnapshot currentSnapshot, IEnvironmentStateProvider environmentStateProvider)
    {
        _environmentStateProvider = environmentStateProvider;
        _formKey = selectedSnapshot.FormKey;
        GetDisplayString();

        var contextMods = selectedSnapshot.ContextSnapshots.Select(x => x.SourceModKey)
            .And(currentSnapshot.ContextSnapshots.Select(x => x.SourceModKey))
            .Distinct()
            .ToArray();

        foreach (var contextMod in contextMods)
        {
            var selectedSnapshotContext = selectedSnapshot.ContextSnapshots.Where(x => x.SourceModKey.Equals(contextMod)).First() ?? new FormContextSnapshot();
            var currentSnapshotContext = currentSnapshot.ContextSnapshots.Where(x => x.SourceModKey.Equals(contextMod)).First() ?? new FormContextSnapshot();
            var contextVM = new VM_FormContextSnapshot(selectedSnapshotContext, currentSnapshotContext, contextMod);
            ContextVMs.Add(contextVM);
        }
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Record;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
    public ObservableCollection<VM_FormContextSnapshot> ContextVMs { get; set; } = new();
    public VM_FormContextSnapshot? SelectedContextVM { get; set; }
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private readonly FormKey _formKey;
    public void GetDisplayString()
    {
        string name = string.Empty;
        string edid = string.Empty;
        if(_environmentStateProvider != null && _environmentStateProvider.LinkCache != null && _environmentStateProvider.LinkCache.TryResolve(_formKey, out var record))
        {
            if(record is INamedGetter named && named.Name != null)
            {
                name = named.Name;
            }
            if(record.EditorID != null)
            {
                edid = record.EditorID;
            }
        }

        DisplayString = string.Join(" | ", name, edid, _formKey.ToString());
    }
}
