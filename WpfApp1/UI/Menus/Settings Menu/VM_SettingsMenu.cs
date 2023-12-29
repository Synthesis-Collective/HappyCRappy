using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_SettingsMenu : VM
{
    public VM_SettingsMenu(IEnvironmentStateProvider environmentStateProvider) 
    {
        EnvironmentStateProvider = environmentStateProvider;
    }
    public IEnvironmentStateProvider EnvironmentStateProvider { get; }
}
