#region

using Core.Camera;
using Core.Input;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Model;
using Feature.Presenter;
using Feature.View;
using Main.Controller;
using Main.Controller.GameNavigation;
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
        [SerializeField] private GameSettings gameSettings;
        
        [SerializeField] private GameObject pausePrefab;
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private GameObject volumesPrefab;
        [SerializeField] private GameObject controlsPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPointExceptionHandler(Debug.Log);
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<TargetGroupManager>();
            builder.RegisterComponentInHierarchy<EnemyFactory>();
            builder.RegisterComponentInHierarchy<SwapEffectFactory>();
            builder.RegisterComponentInHierarchy<InputActionAccessor>();
            builder.RegisterComponentInHierarchy<GameUIView>();
            builder.RegisterComponentInHierarchy<SwapView>();
            builder.RegisterComponentInHierarchy<VoltageBar>();
            builder.RegisterComponentInHierarchy<SelectorEffect>();
            builder.RegisterComponentInHierarchy<VolumeController>();
            builder.RegisterComponentInHierarchy<CameraSwitcher>();
            builder.RegisterComponentInHierarchy<AudioSource>();


            builder.Register<SwapPresenter>(Lifetime.Scoped);
            builder.Register<SwapModel>(Lifetime.Scoped);
            builder.RegisterInstance(characterParams);
            builder.RegisterInstance(gameSettings);

            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<PlayerPresenter>(Lifetime.Scoped);
            builder.Register<IGameController, MainController>(Lifetime.Scoped);
            builder.Register<IInputController, InputController>(Lifetime.Scoped);
            builder.RegisterComponent(pausePrefab.GetComponent<PauseScreen>());
            builder.RegisterComponent(optionPrefab.GetComponent<OptionScreen>());
            builder.RegisterComponent(volumesPrefab.GetComponent<VolumesScreen>());
            builder.RegisterComponent(controlsPrefab.GetComponent<ControlsScreen>());
            builder.RegisterInstance(NavigationExtensions.GetNavigation(
                pausePrefab,
                optionPrefab,
                volumesPrefab,
                controlsPrefab,
                this
            ));

            GameManager.Register(builder);
            builder.RegisterEntryPoint<GameManager>(Lifetime.Scoped);
        }
    }
}