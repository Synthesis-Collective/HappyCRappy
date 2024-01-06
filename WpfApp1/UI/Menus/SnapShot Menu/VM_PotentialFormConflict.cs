using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using ReactiveUI;
using Noggog;

namespace HappyCRappy;

public class VM_PotentialFormConflict: VM, ISnapshotDisplayNode
{
    public delegate VM_PotentialFormConflict Factory(PotentialConflictFinder.PotentialConflictRecord data);
    public VM_PotentialFormConflict(PotentialConflictFinder.PotentialConflictRecord data, IEnvironmentStateProvider environmentStateProvider, VM_SnapshotMenu snapshotMenu, VM_PotentialFormContextConflict.Factory contextConflictFactory)
    {
        _environmentStateProvider = environmentStateProvider;
        _snapshotMenu = snapshotMenu;
        _formKey = data.RecordFormKey;
        GetDisplayString();

        var dataList = data.Serializations.ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j = i + 1; j < dataList.Count; j++)
            {
                ContextPairingVMs.Add(contextConflictFactory(dataList[i], dataList[j], _snapshotMenu.SerializationType));
            }
        }

        UpdateVisibility();

        this.WhenAnyValue(x => x._snapshotMenu.ShowOnlyConflicts).Subscribe(_ => UpdateVisibility()).DisposeWith(this);
    }

    public SnapshotDisplayNodeType NodeType { get; set; } = SnapshotDisplayNodeType.Record;
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; } = new();
    public string DisplayString { get; set; } = string.Empty;
    public bool HasDifference { get; set; } = true;
    public SolidColorBrush BorderColor { get; set; } = new(Colors.Purple);
    public ObservableCollection<VM_PotentialFormContextConflict> ContextPairingVMs { get; set; } = new();
    public VM_PotentialFormContextConflict? SelectedContextPairVM { get; set; }
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private FormKey _formKey { get; set; }
    public bool VisibleChildOrSelf { get; set; }
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

    public void GetDisplayString()
    {
        string name = string.Empty;
        string edid = string.Empty;
        if (_environmentStateProvider != null && _environmentStateProvider.LinkCache != null && _environmentStateProvider.LinkCache.TryResolve(_formKey, out var record))
        {
            if (record is INamedGetter named && named.Name != null)
            {
                name = named.Name;
            }
            if (record.EditorID != null)
            {
                edid = record.EditorID;
            }
        }

        DisplayString = string.Join(" | ", name, edid, _formKey.ToString());
    }
}
