#region

using Core.Camera;
using Core.Input;
using Feature.Common;
using Feature.Model;
using Feature.Presenter;
using Feature.View;
using Main.Controller;
using Main.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#endregion

namespace Main.Installer
{
    public class MainInstaller : LifetimeScope
    {
        [SerializeField] private CharacterParams characterParams;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterEntryPoint<MainController>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<TargetGroupManager>();
            builder.RegisterComponentInHierarchy<EnemyFactory>();
            builder.RegisterComponentInHierarchy<PlayerView>();
            builder.RegisterComponentInHierarchy<InputActionAccessor>();
            builder.RegisterComponentInHierarchy<GameUIView>();

            builder.Register<PlayerPresenter>(Lifetime.Scoped);
            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<SwapPresenter>(Lifetime.Scoped);
            builder.Register<SwapModel>(Lifetime.Scoped);
            builder.RegisterComponent(characterParams);
        }
    }
}