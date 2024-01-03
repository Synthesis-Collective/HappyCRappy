using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HappyCRappy;

public class VM_FormContextSnapshot : VM
{
    public VM_FormContextSnapshot(FormContextSnapshot selectedSnapshot, FormContextSnapshot currentSnapshot, ModKey contextMod)
    {
        ContextModKey = contextMod;
        SelectedSerialization = selectedSnapshot.SerializationString;
        CurrentSerialization = currentSnapshot.SerializationString;
        HasDifference = !CurrentSerialization.Equals(SelectedSerialization);

        if (HasDifference)
        {
            BorderColor = new(Colors.Red);
        }
        else
        {
            BorderColor = new(Colors.White);
        }
    }

    public ModKey ContextModKey { get; set; }
    public bool HasDifference { get; set; } = false;
    public string SelectedSerialization { get; set; }
    public string CurrentSerialization { get; set; }
    public SolidColorBrush BorderColor { get; set; }
}
