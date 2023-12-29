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

namespace HappyCRappy;

public class VM_SettingsMenu : VM
{
    public VM_SettingsMenu(StandaloneEnvironmentStateProvider environmentStateProvider)
    {
        EnvironmentStateProvider = environmentStateProvider;
    }

    public StandaloneEnvironmentStateProvider EnvironmentStateProvider { get; }
}
