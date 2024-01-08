using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Noggog;

namespace HappyCRappy;

public class VM_PotentialFormContextConflict : VM
{
    public delegate VM_PotentialFormContextConflict Factory(KeyValuePair<ModKey, string> serialization1, KeyValuePair<ModKey, string> serialization2, SerializationType serializationType);
    public VM_PotentialFormContextConflict(KeyValuePair<ModKey, string> serialization1, KeyValuePair<ModKey, string> serialization2, SerializationType serializationType, SerializationSwitcher serializationSwitcher, VM_SnapshotMenu snapshotMenu)
    {
        _serializationSwitcher = serializationSwitcher;
        _snapshotMenu = snapshotMenu;

        Serialization1 = serialization1.Value;
        Serialization2 = serialization2.Value;
        ModName1 = serialization1.Key.FileName;
        ModName2 = serialization2.Key.FileName;

        this.WhenAnyValue(x => x._snapshotMenu.SerializationType).Subscribe(newSerializationType =>
        {
            UpdateSerialization(newSerializationType);
        }).DisposeWith(this);

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

        SerializationType1 = serializationType;
        SerializationType2 = serializationType;

        if (_snapshotMenu.SerializationType != SerializationType1 || _snapshotMenu.SerializationType != SerializationType2)
        {
            UpdateSerialization(_snapshotMenu.SerializationType);
        }
    }

    public bool HasDifference { get; set; } = false;
    public string Serialization1 { get; set; }
    public string Serialization2 { get; set; }
    public string ModName1 { get; set; }
    public string ModName2 { get; set; }
    public string DisplayString { get; set; }
    public SolidColorBrush BorderColor { get; set; }
    public SerializationType SerializationType1 { get; set; }
    public SerializationType SerializationType2 { get; set; }
    private readonly SerializationSwitcher _serializationSwitcher;
    private readonly VM_SnapshotMenu _snapshotMenu;

    public void UpdateSerialization(SerializationType newSerializationType)
    {
        Serialization1 = _serializationSwitcher.SwitchSerialization(Serialization1, SerializationType1, newSerializationType, out bool success1, out string exceptionStr1);
        Serialization2 = _serializationSwitcher.SwitchSerialization(Serialization2, SerializationType2, newSerializationType, out bool success2, out string exceptionStr2);

        string errorMessage = string.Empty;
        if (success1)
        {
            SerializationType1 = newSerializationType;
        }
        if (success2)
        {
            SerializationType2 = newSerializationType;
        }

        if (!success1 || !success2)
        {
            errorMessage = string.Join(Environment.NewLine + Environment.NewLine + "//////" + Environment.NewLine + Environment.NewLine, exceptionStr1, exceptionStr2);
        }
    }
}
