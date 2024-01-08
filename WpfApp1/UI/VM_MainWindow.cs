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
    public VM_MainWindow(VM_SettingsMenu settingsVM, VM_IO vmIO, VM_SnapshotMenu snapShotVM)
    {
        _vmIO = vmIO;
        _vmIO.CopyInViewModels();

        _settingsVM = settingsVM;
        _snapShotVM = snapShotVM;

        DisplayedVM = snapShotVM;

        ShowSettingsMenu = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DisplayedVM = _settingsVM;
            }
        );

        ShowSnapShotMenu = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DisplayedVM = _snapShotVM;
            }
        );

        Application.Current.Exit += OnApplicationExit;
    }
    public RelayCommand ShowSettingsMenu { get; }
    public RelayCommand ShowSnapShotMenu { get; }
    public object DisplayedVM { get; set; }
    public static string _programVersion = "1.0.0";
    public string ProgramVersion { get; set; } = _programVersion;

    private readonly VM_SettingsMenu _settingsVM;
    private readonly VM_SnapshotMenu _snapShotVM;


    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        _vmIO.DumpViewModels();
    }
}
