using Noggog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_ModDisplay : VM
{
    public delegate VM_ModDisplay Factory(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot, Dictionary<string, List<PotentialConflictFinder.PotentialConflictRecord>> potentialConflicts);
    public VM_ModDisplay(ModSnapshot selectedSnapshot, ModSnapshot currentSnapshot, Dictionary<string, List<PotentialConflictFinder.PotentialConflictRecord>> potentialConflicts, VM_RecordCategoryDisplay.Factory snapshotGroupFactory, VM_SnapshotMenu snapshotMenu)
    {
        _snapshotMenu = snapshotMenu;

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

            potentialConflicts.TryGetValue(category, out var conflictsInCategory);
            if (conflictsInCategory == null)
            {
                conflictsInCategory = new();
            }
            var categoryVM = snapshotGroupFactory(category, pairedSelectedCurrentSnapshots, conflictsInCategory);
            RecordCategories.Add(categoryVM);
            NumConflict += categoryVM.NumConflict;
            NumNonConflict += categoryVM.NumNonConflict;
            NumPotentialConflict += categoryVM.NumPotentialConflict;
        }

        HasDifference = RecordCategories.Where(x => x.HasDifference).Any();

        ShowPotentialConflictCount = _snapshotMenu.ShowPotentialConflicts;
        this.WhenAnyValue(x => x._snapshotMenu.ShowPotentialConflicts)
            .Subscribe(val => ShowPotentialConflictCount = val)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedNode).Subscribe(y =>
        {
            if (y == null)
            {
                ShowStats = true;
            }
            else
            {
                ShowStats = false;
            }
        }).DisposeWith(this);

        if (NumNonConflict == 1)
        {
            NonConflictStr = "record identical to Snapshot";
        }
        else
        {
            NonConflictStr = "records identical to Snapshot";
        }

        if (NumPotentialConflict == 1)
        {
            PotentialConflictStr = "record which is not a member of the displayed Conflict Resolution mod, but which may need Conflict Resolution";
        }
        else
        {
            PotentialConflictStr = "records which are not members of the displayed Conflict Resolution mod, but which may need Conflict Resolution";
        }

        if (NumConflict == 1)
        {
            ConflictStr = "record with differences from Snapshot";
        }
        else
        {
            ConflictStr = "records with differences from Snapshot";
        }
    }

    public ModSnapshot SelectedSnapshot { get; set; }
    public ModSnapshot CurrentSnapShot { get; set; }
    public DateTime DateTaken { get; set; }
    public ObservableCollection<VM_RecordCategoryDisplay> RecordCategories { get; set; } = new();
    public ISnapshotDisplayNode? SelectedNode { get; set; }
    public string DateTakenStr => ToLabelString(DateTaken);
    public bool HasDifference { get; set; } = false;
    public int NumNonConflict { get; set; } = 0;
    public string NonConflictStr { get; set; }
    public int NumConflict { get; set; } = 0;
    public string ConflictStr { get; set; }
    public int NumPotentialConflict { get; set; } = 0;
    public string PotentialConflictStr { get; set; }
    public bool ShowPotentialConflictCount { get; set; } = false;
    public bool ShowStats { get; set; } = false;
    private readonly VM_SnapshotMenu _snapshotMenu;

    public static string ToLabelString(DateTime timestamp)
    {
        return $"{timestamp:yyyy-MM-dd-HH-mm-ss}";
    }
}
