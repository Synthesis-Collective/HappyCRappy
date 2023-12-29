using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HappyCRappy;

public class VM_MainWindow : VM
{
    private readonly VM_IO _vmIO;
    public VM_MainWindow(VM_SettingsMenu settingsVM, VM_IO vmIO)
    {
        _vmIO = vmIO;
        _vmIO.CopyInViewModels();

        _settingsVM = settingsVM;

        DisplayedVM = _settingsVM;

        ShowSettingsMenu = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DisplayedVM = _settingsVM;
            }
        );

        Application.Current.Exit += OnApplicationExit;
    }
    public RelayCommand ShowSettingsMenu { get; }
    public object DisplayedVM { get; set; }

    private readonly VM_SettingsMenu _settingsVM;

    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        _vmIO.DumpViewModels();
    }
}
