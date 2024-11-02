using Core.Navigation;
using Main.Installer;
using UnityEngine;

namespace Main.Controller.GameNavigation
{
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
                    var instance = Object.Instantiate(destination.Content).GetComponent<AScreen>();
                    installer.Container.Inject(instance);
                    return instance;
                }, 
                new Destination<Navigation>(Navigation.Pause, pausePrefab),
                new Destination<Navigation>(Navigation.Option, optionPrefab),
                new Destination<Navigation>(Navigation.Volumes, volumesPrefab),
                new Destination<Navigation>(Navigation.Controls, controlsPrefab)
            );
        }
    }
}