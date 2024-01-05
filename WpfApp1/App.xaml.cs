using Autofac;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Mutagen.Bethesda.Synthesis.WPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace HappyCRappy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        BindingErrorListener.Register(); // for debugging

        SynthesisPipeline.Instance
            //.SetOpenForSettings(OpenForSettings)
            //.AddRunnabilityCheck(CanRunPatch)
            //.AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)sz
            .SetTypicalOpen(StandaloneOpen)
            .SetForWpf()
            .Run(e.Args)
            .Wait();
    }

    public int StandaloneOpen()
    {
        var window = new MainWindow();

        var builder = new ContainerBuilder();
        builder.RegisterModule<MainModule>();
        var container = builder.Build();
        var mainVM = container.Resolve<VM_MainWindow>();

        window.DataContext = mainVM;
        window.Show();

        return 0;
    }
}
