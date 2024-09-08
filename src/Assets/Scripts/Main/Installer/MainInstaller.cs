#region

using Core.Camera;
using Core.Input;
using Feature.Common.Parameter;
using Feature.Component.Factory;
using Feature.Interface;
using Feature.Model;
using Feature.Presenter;
using Feature.View;
using Main.Controller;
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
            builder.RegisterEntryPointExceptionHandler(Debug.Log);
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<TargetGroupManager>();
            builder.RegisterComponentInHierarchy<EnemyFactory>();
            builder.RegisterComponentInHierarchy<InputActionAccessor>();
            builder.RegisterComponentInHierarchy<GameUIView>();
            builder.RegisterComponentInHierarchy<SwapView>();

            builder.Register<SwapPresenter>(Lifetime.Scoped);
            builder.Register<SwapModel>(Lifetime.Scoped);
            builder.RegisterComponent(characterParams);

            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<PlayerPresenter>(Lifetime.Scoped);
            builder.Register<IGameController, MainController>(Lifetime.Scoped);

            GameManager.Register(builder);
            builder.RegisterEntryPoint<GameManager>(Lifetime.Scoped);
        }
    }
}