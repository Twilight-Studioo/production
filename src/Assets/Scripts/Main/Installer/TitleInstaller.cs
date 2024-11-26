using Feature.Interface;
using Main.Controller;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class TitleInstaller: LifetimeScope
    {

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IOutGameController, TitleController>(Lifetime.Scoped);

            builder.RegisterEntryPoint<TitleManager>(Lifetime.Scoped);
        }
    }
}