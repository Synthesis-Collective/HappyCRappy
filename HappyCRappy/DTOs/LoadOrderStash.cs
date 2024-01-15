using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class LoadOrderStash
{
    public List<LoadOrderBlock> ModChunks { get; set; } = new();
    public DateTime DateTaken { get; set; }
    public string Version { get; set; } = VM_MainWindow._programVersion;
}
