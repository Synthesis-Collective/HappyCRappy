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
    public VM_MainWindow(VM_SettingsMenu settingsVM, VM_IO vmIO, VM_SnapshotMenu snapShotVM, VM_LoadOrderMenu loadOrderVM, LoadOrderStashRestorer loadOrderRestorer)
    {
        _vmIO = vmIO;
        _vmIO.CopyInViewModels();

        _settingsVM = settingsVM;
        _snapShotVM = snapShotVM;
        _loadOrderVM = loadOrderVM;
        _loadOrderVM.Initialize();

        _loadOrderRestorer = loadOrderRestorer;

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

        ShowLoadOrderMenu = new RelayCommand(
            canExecute: _ => true,
            execute: _ =>
            {
                DisplayedVM = _loadOrderVM;
            }
        );

        Application.Current.Exit += OnApplicationExit;
    }
    public RelayCommand ShowSettingsMenu { get; }
    public RelayCommand ShowSnapShotMenu { get; }
    public RelayCommand ShowLoadOrderMenu { get; }
    public object DisplayedVM { get; set; }
    public static string _programVersion = "1.0.0";
    public string ProgramVersion { get; set; } = _programVersion;

    private readonly VM_SettingsMenu _settingsVM;
    private readonly VM_SnapshotMenu _snapShotVM;
    private readonly VM_LoadOrderMenu _loadOrderVM;
    private readonly LoadOrderStashRestorer _loadOrderRestorer;


    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        _vmIO.DumpViewModels();

        if(_loadOrderVM.ToggleApplyLoadOrderStash && _loadOrderVM.StashToApply != null)
        {
            _loadOrderRestorer.ApplyStash(_loadOrderVM.StashToApply, _loadOrderVM.LoadOrder.Select(x => x.ModKey).ToList());
        }
    }
}
