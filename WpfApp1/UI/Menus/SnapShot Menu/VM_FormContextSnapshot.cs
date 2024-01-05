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

public class VM_FormContextSnapshot : VM
{
    public delegate VM_FormContextSnapshot Factory(FormContextSnapshot selectedSnapshot, FormContextSnapshot currentSnapshot, ModKey contextMod);
    public VM_FormContextSnapshot(FormContextSnapshot selectedSnapshot, FormContextSnapshot currentSnapshot, ModKey contextMod, VM_SnapshotMenu snapshotMenu, SerializationSwitcher serializationSwitcher)
    {
        _serializationSwitcher = serializationSwitcher;

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

        SelectedSerializationType = selectedSnapshot.SerializationType;
        CurrentSerializationType = currentSnapshot.SerializationType;

        if(snapshotMenu.SerializationType != CurrentSerializationType || snapshotMenu.SerializationType != SelectedSerializationType)
        {
            UpdateSerialization(snapshotMenu.SerializationType);
        }

        this.WhenAnyValue(x => snapshotMenu.SerializationType).Subscribe(newSerializationType =>
        {
            UpdateSerialization(newSerializationType);
        }).DisposeWith(this);
    }

    public ModKey ContextModKey { get; set; }
    public bool HasDifference { get; set; } = false;
    public string SelectedSerialization { get; set; }
    public string CurrentSerialization { get; set; }
    public SerializationType SelectedSerializationType { get; set; }
    public SerializationType CurrentSerializationType { get; set; }
    public SolidColorBrush BorderColor { get; set; }
    private readonly SerializationSwitcher _serializationSwitcher;

    public void UpdateSerialization(SerializationType newSerializationType)
    {
        SelectedSerialization = _serializationSwitcher.SwitchSerialization(SelectedSerialization, SelectedSerializationType, newSerializationType, out bool success1, out string exceptionStr1);
        CurrentSerialization = _serializationSwitcher.SwitchSerialization(CurrentSerialization, CurrentSerializationType, newSerializationType, out bool success2, out string exceptionStr2);

        string errorMessage = string.Empty;
        if (success1)
        {
            SelectedSerializationType = newSerializationType;
        }
        if (success2)
        {
            CurrentSerializationType = newSerializationType;
        }

        if (!success1 || !success2)
        {
            errorMessage = string.Join(Environment.NewLine + Environment.NewLine + "//////" + Environment.NewLine + Environment.NewLine, exceptionStr1, exceptionStr2);
        }
    }
}
