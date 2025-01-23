using Feature.Interface;
using Main.Controller.SmasherClear;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class SmasherClearInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IOutGameController, SmasherClearController>(Lifetime.Scoped);

            builder.RegisterEntryPoint<SmasherClearManager>(Lifetime.Scoped);
        }
    }
}