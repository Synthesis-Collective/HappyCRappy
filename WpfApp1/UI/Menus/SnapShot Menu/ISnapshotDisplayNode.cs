using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HappyCRappy;

public interface ISnapshotDisplayNode
{
    public SnapshotDisplayNodeType NodeType { get; set; }
    public ObservableCollection<ISnapshotDisplayNode> SubNodes { get; set; }
    public string DisplayString { get; set; }
    public bool HasDifference { get; set; }
    public SolidColorBrush BorderColor { get; set; }
}
