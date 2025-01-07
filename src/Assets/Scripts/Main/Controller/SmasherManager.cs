using System.Runtime.InteropServices;
using Core.Camera;
using Feature.Component.Enemy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Controller
{
    public class SmasherManager : IStartable
    {
        private readonly TargetGroupManager targetGroupManager;
        private readonly Smasher smasher;

        [Inject]
        public SmasherManager(TargetGroupManager targetGroupManager,Smasher smasher)
        {
            this.targetGroupManager = targetGroupManager;
            this.smasher = smasher;
        }

        public void Start()
        {
            targetGroupManager.AddTarget(smasher.transform,CameraTargetGroupTag.Boss());
        }
        
    }
}