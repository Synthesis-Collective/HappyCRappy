using Noggog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_Snapshot : VM
{
    public delegate VM_Snapshot Factory(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot);
    public VM_Snapshot(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot, VM_CategorySnapshot.Factory snapshotGroupFactory)
    {
        SelectedSnapshot = selectedSnapshot;
        CurrentSnapShot = currentSnapshot;
        DateTaken = selectedSnapshot.DateTaken;

        var categories = selectedSnapshot.Snapshots.Select(x => x.RecordType)
            .And(currentSnapshot.Snapshots.Select(y => y.RecordType))
            .Distinct();

        foreach (var category in categories)
        {
            List<(FormSnapshot, FormSnapshot)> pairedSelectedCurrentSnapshots = new();

            var selectedForms = selectedSnapshot.Snapshots.Where(x => x.RecordType == category).Select(x => x.FormKey).ToArray();
            var currentForms = currentSnapshot.Snapshots.Where(x => x.RecordType == category).Select(x => x.FormKey).ToArray();
            var allForms = selectedForms.And(currentForms).Distinct().ToArray();

            foreach (var formKey in allForms)
            {
                var selectedFormSnapshot = selectedSnapshot.Snapshots.Where(x => x.FormKey.Equals(formKey)).First() ?? new FormSnapshot();
                var currentFormSnapshot = currentSnapshot.Snapshots.Where(x => x.FormKey.Equals(formKey)).First() ?? new FormSnapshot();
                pairedSelectedCurrentSnapshots.Add((selectedFormSnapshot, currentFormSnapshot));
            }

            var categoryVM = snapshotGroupFactory(category, pairedSelectedCurrentSnapshots);
            RecordCategories.Add(categoryVM);
        }
    }

    public ModSnapshot SelectedSnapshot { get; set; }
    public ModSnapshot CurrentSnapShot { get; set; }
    public DateTime DateTaken { get; set; }
    public ObservableCollection<ISnapshotDisplayNode> RecordCategories { get; set; } = new();
    public ISnapshotDisplayNode? SelectedNode { get; set; }
    public string DateTakenStr => ToLabelString(DateTaken);

    public static string ToLabelString(DateTime timestamp)
    {
        return $"{timestamp:yyyy-MM-dd-HH-mm-ss}";
    }
}
