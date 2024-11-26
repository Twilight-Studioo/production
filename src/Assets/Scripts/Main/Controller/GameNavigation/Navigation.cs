#region

using Core.Navigation;
using Core.Utilities;
using UnityEngine;
using VContainer.Unity;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     ゲーム画面内のナビゲーション状態を定義します
    /// </summary>
    public enum Navigation
    {
        Pause,
        Option,
        Volumes,
        Controls,
        Title,
        GameOver,
        StageSelect,
    }

    public static class NavigationExtensions
    {
        public static ScreenController<Navigation> GetNavigation(
            GameObject pausePrefab,
            GameObject optionPrefab,
            GameObject volumesPrefab,
            GameObject controlsPrefab,
            GameObject titlePrefab,
            GameObject gameOverPrefab,
            GameObject stageSelectPrefab,
            LifetimeScope installer
        )
        {
            return ScreenController<Navigation>.Create(
                destination =>
                {
                    var instance = Object.Instantiate(destination.Content).GetComponent<IScreen>();
                    installer.Container.Inject(instance);
                    return instance;
                },
                new Destination<Navigation>(Navigation.Pause, pausePrefab.CheckNull()),
                new Destination<Navigation>(Navigation.Option, optionPrefab.CheckNull()),
                new Destination<Navigation>(Navigation.Volumes, volumesPrefab.CheckNull()),
                new Destination<Navigation>(Navigation.Controls, controlsPrefab.CheckNull()),
                new Destination<Navigation>(Navigation.Title, titlePrefab.CheckNull()),
                new Destination<Navigation>(Navigation.GameOver, gameOverPrefab.CheckNull()),
                new Destination<Navigation>(Navigation.StageSelect, stageSelectPrefab.CheckNull())
            );
        }
    }
}