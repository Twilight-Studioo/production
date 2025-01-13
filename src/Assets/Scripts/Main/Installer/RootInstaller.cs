using Core.Input;
using Core.Utilities;
using Feature.Common.Parameter;
using Main.Controller;
using Main.Controller.GameNavigation;
using Main.Controller.Save;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class RootInstaller : LifetimeScope
    {
        [SerializeField] private GameObject pausePrefab;
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private GameObject volumesPrefab;
        [SerializeField] private GameObject controlsPrefab;
        [SerializeField] private GameObject titlePrefab;
        [SerializeField] private GameObject gameOverPrefab;
        [SerializeField] private GameObject stageSelectPrefab;
        [SerializeField] private GameObject smasherClearPrefab;

        [SerializeField] private GameSettings settings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<InputActionAccessor>();
            builder.RegisterInstance(NavigationExtensions.GetNavigation(
                pausePrefab,
                optionPrefab,
                volumesPrefab,
                controlsPrefab,
                titlePrefab,
                gameOverPrefab,
                smasherClearPrefab,
                stageSelectPrefab,
                this
            ).CheckNull());

            builder.RegisterInstance(settings);

            builder.RegisterComponent(pausePrefab.GetComponent<PauseScreen>().CheckNull());
            builder.RegisterComponent(optionPrefab.GetComponent<OptionScreen>().CheckNull());
            builder.RegisterComponent(volumesPrefab.GetComponent<VolumesScreen>().CheckNull());
            builder.RegisterComponent(controlsPrefab.GetComponent<ControlsScreen>().CheckNull());
            builder.RegisterComponent(titlePrefab.GetComponent<TitleScreen>().CheckNull());
            builder.RegisterComponent(gameOverPrefab.GetComponent<GameOverScreen>().CheckNull());
            builder.RegisterComponent(smasherClearPrefab.GetComponent<ClearSmasherScreen>().CheckNull());
            builder.Register<GameSaveManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<RootManager>();
        }
    }
}