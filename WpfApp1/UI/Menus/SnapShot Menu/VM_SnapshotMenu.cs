using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_SnapshotMenu : VM
{
    private readonly SettingsProvider _settingsProvider;
    public VM_SnapshotMenu(SettingsProvider settingsProvider, SnapShotter snapShotter) 
    {
        _settingsProvider = settingsProvider;

        TakeSnapShot = new RelayCommand(
                canExecute: _ => true,
                execute: _ =>
                {
                    snapShotter.TakeSnapShot();
                }
            );
    }

    public RelayCommand TakeSnapShot { get; }
}
