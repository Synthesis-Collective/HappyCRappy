using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_FormContextSnapshot : VM
{
    public VM_FormContextSnapshot(FormContextSnapshot selectedSnapshot, FormContextSnapshot currentSnapshot, ModKey contextMod)
    {
        ContextModKey = contextMod;
    }

    public ModKey ContextModKey { get; set; }
    public bool HasDifference { get; set; } = false;
    public string htmlDisplayStr { get; set; } = string.Empty;
}
