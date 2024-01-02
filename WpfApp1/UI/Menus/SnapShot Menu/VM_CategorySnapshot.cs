using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using System.Reactive.Disposables;
using Noggog;

namespace HappyCRappy;

public class VM_CategorySnapshot : VM, ISnapshotDisplayNode
{
    public VM_CategorySnapshot(string recordType, List<(FormSnapshot, FormSnapshot)> snapshots)
    {
        RecordType = recordType;

        foreach (var snapshot in snapshots)
        {
            var displayedRecord = new VM_FormSnapshot(snapshot.Item1, snapshot.Item2);
            SubNodes.Add(displayedRecord);
        }
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Category;
    public string RecordType { get; set; }
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
}
