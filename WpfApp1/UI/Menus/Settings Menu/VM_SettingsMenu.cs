using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Noggog;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;

namespace HappyCRappy;

public class VM_SettingsMenu : VM
{
    public VM_SettingsMenu(IEnvironmentStateProvider environmentStateProvider)
    {
        EnvironmentStateProvider = environmentStateProvider;
    }

    public IEnvironmentStateProvider EnvironmentStateProvider { get; }
    public ObservableCollection<ModKey> TrackedModKeys { get; set; } = new();

    public void ReadFromModel(HappyCrappySettings model)
    {
        TrackedModKeys = new(model.TrackedModKeys);

        if (EnvironmentStateProvider is StandaloneEnvironmentStateProvider standaloneEnvironment)
        {
            standaloneEnvironment.GameType = model.GameType;
            standaloneEnvironment.DataFolderPath = model.DataFolderPath;
        }
    }

    public HappyCrappySettings DumpToModel()
    {
        HappyCrappySettings model = new()
        {
            GameType = EnvironmentStateProvider.GameType,
            DataFolderPath = EnvironmentStateProvider.DataFolderPath,
            TrackedModKeys = TrackedModKeys.ToList()
        };
        return model;
    }
}
