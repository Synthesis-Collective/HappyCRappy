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
    public delegate VM_CategorySnapshot Factory(string recordType, List<(FormSnapshot, FormSnapshot)> snapshots);
    public VM_CategorySnapshot(string recordType, List<(FormSnapshot, FormSnapshot)> snapshots, VM_FormSnapshot.Factory formSnapshotFactory)
    {
        DisplayString = recordType;

        foreach (var snapshot in snapshots)
        {
            var displayedRecord = formSnapshotFactory(snapshot.Item1, snapshot.Item2);
            SubNodes.Add(displayedRecord);
        }
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Category;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
}
