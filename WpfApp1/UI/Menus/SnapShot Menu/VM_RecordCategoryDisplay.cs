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
using System.Windows.Media;

namespace HappyCRappy;

public class VM_RecordCategoryDisplay : VM, ISnapshotDisplayNode
{
    public delegate VM_RecordCategoryDisplay Factory(string recordType, List<(FormSnapshot, FormSnapshot)> snapshots, List<PotentialConflictFinder.PotentialConflictRecord> potentialConflicts);
    public VM_RecordCategoryDisplay(string recordType, List<(FormSnapshot, FormSnapshot)> snapshots, List<PotentialConflictFinder.PotentialConflictRecord> potentialConflicts, VM_FormSnapshot.Factory formSnapshotFactory, VM_PotentialFormConflict.Factory potentialConflictFactory, VM_SnapshotMenu snapshotMenu)
    {
        _snapshotMenu = snapshotMenu;

        DisplayString = recordType;

        foreach (var snapshot in snapshots)
        {
            var displayedRecord = formSnapshotFactory(snapshot.Item1, snapshot.Item2);
            SubNodes.Add(displayedRecord);
        }

        foreach (var potentialConflict in potentialConflicts)
        {
            var potentialConflictVM = potentialConflictFactory(potentialConflict);
            SubNodes.Add(potentialConflictVM);
        }

        HasDifference = SubNodes.Where(x => x.HasDifference).Any();

        if (HasDifference)
        {
            BorderColor = new(Colors.Red);
        }
        else
        {
            BorderColor = new(Colors.White);
        }

        UpdateVisibility();

        this.WhenAnyValue(x => x._snapshotMenu.ShowOnlyConflicts).Subscribe(_ => UpdateVisibility()).DisposeWith(this);

        ShowPotentialConflictCount = _snapshotMenu.ShowPotentialConflicts;
        this.WhenAnyValue(x => x._snapshotMenu.ShowPotentialConflicts)
            .Subscribe(val => ShowPotentialConflictCount = val)
            .DisposeWith(this);

        NumNonConflict = SubNodes.Where(x => !x.HasDifference).Count();
        if (NumNonConflict == 1)
        {
            NonConflictStr = "record identical to Snapshot";
        }
        else
        {
            NonConflictStr = "records identical to Snapshot";
        }

        NumPotentialConflict = potentialConflicts.Count;
        if (NumPotentialConflict == 1)
        {
            PotentialConflictStr = "record which is not a member of the displayed Conflict Resolution mod, but which may need Conflict Resolution";
        }
        else
        {
            PotentialConflictStr = "records which are not members of the displayed Conflict Resolution mod, but which may need Conflict Resolution";
        }

        NumConflict = SubNodes.Count - NumConflict - NumPotentialConflict;
        if (NumConflict == 1)
        {
            ConflictStr = "record with differences from Snapshot";
        }
        else
        {
            ConflictStr = "records with differences from Snapshot";
        }
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Category;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
    public bool HasDifference { get; set; } = false;
    public SolidColorBrush BorderColor { get; set; }
    public bool VisibleChildOrSelf { get; set; }
    public int NumNonConflict { get; set; } = 0;
    public string NonConflictStr { get; set; }
    public int NumConflict { get; set; } = 0;
    public string ConflictStr { get; set; }
    public int NumPotentialConflict { get; set; } = 0;
    public string PotentialConflictStr { get; set; }
    public bool ShowPotentialConflictCount { get; set; } = false;
    private readonly VM_SnapshotMenu _snapshotMenu;

    private void UpdateVisibility()
    {
        if (!_snapshotMenu.ShowOnlyConflicts || HasDifference)
        {
            VisibleChildOrSelf = true;
        }
        else
        {
            VisibleChildOrSelf = false;
        }
    }
}
