using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class ModSnapshot
{
    public ModKey CRModKey { get; set; }
    public DateTime DateTaken { get; set; }
    public List<FormSnapshot> SnapshotsByType { get; set; } = new();
}
