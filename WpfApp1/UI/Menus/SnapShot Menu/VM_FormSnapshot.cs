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
using System.Windows.Media;
using ReactiveUI;

namespace HappyCRappy;

public class VM_FormSnapshot : VM, ISnapshotDisplayNode
{
    public delegate VM_FormSnapshot Factory(FormSnapshot selectedSnapshot, FormSnapshot currentSnapshot);
    public VM_FormSnapshot(FormSnapshot selectedSnapshot, FormSnapshot currentSnapshot, IEnvironmentStateProvider environmentStateProvider, VM_FormContextSnapshot.Factory contextSnapshotFactory, VM_SnapshotMenu snapshotMenu)
    {
        _environmentStateProvider = environmentStateProvider;
        _snapshotMenu = snapshotMenu;
        _formKey = selectedSnapshot.FormKey;
        GetDisplayString();

        var contextMods = selectedSnapshot.ContextSnapshots.Select(x => x.SourceModKey)
            .And(currentSnapshot.ContextSnapshots.Select(x => x.SourceModKey))
            .Distinct()
            .ToArray();

        foreach (var contextMod in contextMods)
        {
            var selectedSnapshotContext = selectedSnapshot.ContextSnapshots.Where(x => x.SourceModKey.Equals(contextMod)).FirstOrDefault() ?? new FormContextSnapshot();
            var currentSnapshotContext = currentSnapshot.ContextSnapshots.Where(x => x.SourceModKey.Equals(contextMod)).FirstOrDefault() ?? new FormContextSnapshot();
            var contextVM = contextSnapshotFactory(selectedSnapshotContext, currentSnapshotContext, contextMod);
            ContextVMs.Add(contextVM);
        }

        SnapshotContextOrder = string.Join(Environment.NewLine, selectedSnapshot.OverrideOrder.Select(x => x.FileName));
        CurrentContextOrder = string.Join(Environment.NewLine, currentSnapshot.OverrideOrder.Select(x => x.FileName));

        HasDifference = ContextVMs.Where(x => x.HasDifference).Any();

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
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Record;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
    public string SnapshotContextOrder { get; set; }
    public string CurrentContextOrder { get; set; }
    public ObservableCollection<VM_FormContextSnapshot> ContextVMs { get; set; } = new();
    public VM_FormContextSnapshot? SelectedContextVM { get; set; }
    public bool HasDifference { get; set; } = false;
    public SolidColorBrush BorderColor { get; set; }
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private readonly VM_SnapshotMenu _snapshotMenu;
    private readonly FormKey _formKey;
    public bool VisibleChildOrSelf { get; set; }

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
