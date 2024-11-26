using Core.Camera;
using Core.Utilities;
using Feature.Component.Enemy;
using Main.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Installer
{
    public class SmasherInstaller : LifetimeScope
    {
        [SerializeField] private Smasher smasher;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(smasher.CheckNull());
            builder.RegisterComponentInHierarchy<TargetGroupManager>();
            builder.RegisterEntryPoint<SmasherManager>(Lifetime.Scoped);
        }
    }
}