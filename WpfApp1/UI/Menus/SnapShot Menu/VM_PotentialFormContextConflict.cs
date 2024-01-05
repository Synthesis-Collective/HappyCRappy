using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
namespace HappyCRappy;

public class VM_PotentialFormContextConflict
{
    public VM_PotentialFormContextConflict(KeyValuePair<ModKey, string> serialization1, KeyValuePair<ModKey, string> serialization2)
    {
        Serialization1 = serialization1.Value;
        Serialization2 = serialization2.Value;
        ModName1 = serialization1.Key.FileName;
        ModName2 = serialization2.Key.FileName;

        HasDifference = !Serialization1.Equals(Serialization2);

        if (HasDifference)
        {
            BorderColor = new(Colors.Red);
        }
        else
        {
            BorderColor = new(Colors.White);
        }

        DisplayString = string.Join(Environment.NewLine, ModName1, "vs.", ModName2);
    }

    public bool HasDifference { get; set; } = false;
    public string Serialization1 { get; set; }
    public string Serialization2 { get; set; }
    public string ModName1 { get; set; }
    public string ModName2 { get; set; }
    public string DisplayString { get; set; }
    public SolidColorBrush BorderColor { get; set; }
}
