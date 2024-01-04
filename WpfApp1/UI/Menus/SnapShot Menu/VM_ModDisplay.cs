using Noggog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_ModDisplay : VM
{
    public delegate VM_ModDisplay Factory(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot);
    public VM_ModDisplay(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot, VM_RecordCategoryDisplay.Factory snapshotGroupFactory)
    {
        SelectedSnapshot = selectedSnapshot;
        CurrentSnapShot = currentSnapshot;
        DateTaken = selectedSnapshot.DateTaken;

        var categories = selectedSnapshot.SnapshotsByType.Select(x => x.RecordType)
            .And(currentSnapshot.SnapshotsByType.Select(y => y.RecordType))
            .Distinct();

        foreach (var category in categories)
        {
            List<(FormSnapshot, FormSnapshot)> pairedSelectedCurrentSnapshots = new();

            var selectedForms = selectedSnapshot.SnapshotsByType.Where(x => x.RecordType == category).Select(x => x.FormKey).ToArray();
            var currentForms = currentSnapshot.SnapshotsByType.Where(x => x.RecordType == category).Select(x => x.FormKey).ToArray();
            var allForms = selectedForms.And(currentForms).Distinct().ToArray();

            foreach (var formKey in allForms)
            {
                var selectedFormSnapshot = selectedSnapshot.SnapshotsByType.Where(x => x.FormKey.Equals(formKey)).FirstOrDefault() ?? new FormSnapshot();
                var currentFormSnapshot = currentSnapshot.SnapshotsByType.Where(x => x.FormKey.Equals(formKey)).FirstOrDefault() ?? new FormSnapshot();
                pairedSelectedCurrentSnapshots.Add((selectedFormSnapshot, currentFormSnapshot));
            }

            var categoryVM = snapshotGroupFactory(category, pairedSelectedCurrentSnapshots);
            RecordCategories.Add(categoryVM);
        }

        HasDifference = RecordCategories.Where(x => x.HasDifference).Any();
    }

    public ModSnapshot SelectedSnapshot { get; set; }
    public ModSnapshot CurrentSnapShot { get; set; }
    public DateTime DateTaken { get; set; }
    public ObservableCollection<ISnapshotDisplayNode> RecordCategories { get; set; } = new();
    public ISnapshotDisplayNode? SelectedNode { get; set; }
    public string DateTakenStr => ToLabelString(DateTaken);
    public bool HasDifference { get; set; } = false;

    public static string ToLabelString(DateTime timestamp)
    {
        return $"{timestamp:yyyy-MM-dd-HH-mm-ss}";
    }
}
