using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class LoadOrderBlock
{
    public List<ModKey> Mods { get; set; } = new();
    public ModKey? PlaceAfter { get; set; }
    public ModKey? PlaceBefore { get; set; }
}
