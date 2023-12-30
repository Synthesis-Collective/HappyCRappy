using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

internal class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<VM_MainWindow>().AsSelf().SingleInstance();
        builder.RegisterType<SettingsProvider>().AsSelf().SingleInstance();
        builder.RegisterType<VM_IO>().AsSelf().SingleInstance();
        builder.RegisterType<StandaloneEnvironmentStateProvider>().AsImplementedInterfaces().AsSelf().SingleInstance();
        builder.RegisterType<VM_SettingsMenu>().AsSelf().SingleInstance();
        builder.RegisterType<VM_SnapshotMenu>().AsSelf().SingleInstance();

        builder.RegisterType<SnapShotter>().AsSelf().SingleInstance();
    }
}
