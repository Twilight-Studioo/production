using Feature.Interface;
using Main.Controller.GameOver;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class GameOverInstaller: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IOutGameController, GameOverController>(Lifetime.Scoped);

            builder.RegisterEntryPoint<GameOverManager>(Lifetime.Scoped);
        }
    }
}