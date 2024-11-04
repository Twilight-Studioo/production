#region

using Core.Navigation;
using Core.Utilities;
using Main.Installer;
using UnityEngine;

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
    }

    public static class NavigationExtensions
    {
        public static ScreenController<Navigation> GetNavigation(
            GameObject pausePrefab,
            GameObject optionPrefab,
            GameObject volumesPrefab,
            GameObject controlsPrefab,
            MainInstaller installer
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
                new Destination<Navigation>(Navigation.Controls, controlsPrefab.CheckNull())
            );
        }
    }
}