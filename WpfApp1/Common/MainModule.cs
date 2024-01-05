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

        builder.RegisterType<VM_ModDisplay>().AsSelf();
        builder.RegisterType<VM_RecordCategoryDisplay>().AsSelf();
        builder.RegisterType<VM_FormSnapshot>().AsSelf();
        builder.RegisterType<VM_FormContextSnapshot>().AsSelf();
        builder.RegisterType<VM_PotentialFormConflict>().AsSelf();
        builder.RegisterType<VM_PotentialFormContextConflict>().AsSelf();

        builder.RegisterType<SnapShotter>().AsSelf().SingleInstance();
        builder.RegisterType<PotentialConflictFinder>().AsSelf().SingleInstance();
        builder.RegisterType<Serializer>().AsSelf().SingleInstance();
        builder.RegisterType<SerializationSwitcher>().AsSelf().SingleInstance();
    }
}
