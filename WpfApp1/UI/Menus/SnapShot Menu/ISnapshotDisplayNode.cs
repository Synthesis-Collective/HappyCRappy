using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public interface ISnapshotDisplayNode
{
    public SnapshotDisplayNodeType NodeType { get; set; }
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; }
    public string DisplayString { get; set; }
}
