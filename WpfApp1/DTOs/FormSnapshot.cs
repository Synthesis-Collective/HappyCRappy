using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class FormSnapshot
{
    public FormKey FormKey { get; set; }
    public List<ModKey> OverrideOrder { get; set; } = new();
    public List<FormContextSnapshot> ContextSnapshots { get; set; } = new();
}
