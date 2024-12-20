using Feature.Component;
using Feature.Interface;
using Main.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class TitleInstaller: LifetimeScope
    {
        [SerializeField] private CinemachineSmoothPathManager cinemachineSmoothPathManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IOutGameController, TitleController>(Lifetime.Scoped);
            builder.RegisterComponent(cinemachineSmoothPathManager);

            builder.RegisterEntryPoint<TitleManager>(Lifetime.Scoped);
        }
    }
}