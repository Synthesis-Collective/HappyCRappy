using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HappyCRappy;

class VM_FormSnapshot : VM, ISnapshotDisplayNode
{
    public VM_FormSnapshot(FormSnapshot selectedSnapshot, FormSnapshot referenceSnapshot)
    {

    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Record;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
}
