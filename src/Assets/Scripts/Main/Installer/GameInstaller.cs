#region

using Core.Camera;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Model;
using Feature.Presenter;
using Feature.View;
using Main.Controller;
using Main.Controller.Save;
using Main.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#endregion

namespace Main.Installer
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private CharacterParams characterParams;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private GameAudioAssets gameAudioAssets;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPointExceptionHandler(Debug.Log);

            builder.RegisterComponentInHierarchy<TargetGroupManager>();
            builder.RegisterComponentInHierarchy<EnemyFactory>();
            builder.RegisterComponentInHierarchy<SwapEffectFactory>();
            builder.RegisterComponentInHierarchy<GameUIView>();
            builder.RegisterComponentInHierarchy<VoltageBar>();
            builder.RegisterComponentInHierarchy<SelectorEffect>();
            builder.RegisterComponentInHierarchy<VolumeController>();
            builder.RegisterComponentInHierarchy<CameraSwitcher>();
            builder.RegisterComponentInHierarchy<AudioSource>();
            builder.RegisterComponentInHierarchy<IAudioMixerController>();


            builder.Register<SwapPresenter>(Lifetime.Scoped);
            builder.Register<SwapModel>(Lifetime.Scoped);
            builder.Register<IEndFieldController, EndFieldController>(Lifetime.Scoped);
            builder.RegisterInstance(characterParams);
            builder.RegisterInstance(gameSettings);
            builder.RegisterInstance(gameAudioAssets);

            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<PlayerPresenter>(Lifetime.Scoped);
            builder.Register<IGameController, GameController>(Lifetime.Scoped);
            builder.Register<IGameInputController, GameInputController>(Lifetime.Scoped);

            GameManager.Register(builder);
            builder.RegisterEntryPoint<GameManager>(Lifetime.Scoped);
        }
    }
}