using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;

namespace HappyCRappy;

public class VM_FormSnapshot : VM, ISnapshotDisplayNode
{
    public delegate VM_FormSnapshot Factory(FormSnapshot selectedSnapshot, FormSnapshot referenceSnapshot);
    public VM_FormSnapshot(FormSnapshot selectedSnapshot, FormSnapshot referenceSnapshot, IEnvironmentStateProvider environmentStateProvider)
    {
        _environmentStateProvider = environmentStateProvider;
        _formKey = selectedSnapshot.FormKey;
        GetDisplayString();
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Record;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
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
