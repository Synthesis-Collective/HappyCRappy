using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HappyCRappy;

public class VM_MainWindow : VM
{
    public VM_MainWindow(VM_SettingsMenu settingsVM)
    {
        _settingsVM = settingsVM;

        DisplayedVM = _settingsVM;

        ShowSettingsMenu = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DisplayedVM = _settingsVM;
            }
        );
    }
    public RelayCommand ShowSettingsMenu { get; }
    public VM DisplayedVM { get; set; }

    private readonly VM_SettingsMenu _settingsVM;
}
