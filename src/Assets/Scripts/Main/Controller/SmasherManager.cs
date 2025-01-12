using Core.Camera;
using Feature.Component.Enemy;
using VContainer;
using VContainer.Unity;

namespace Main.Controller
{
    public class SmasherManager : IStartable
    {
        private readonly Smasher smasher;
        private readonly TargetGroupManager targetGroupManager;

        [Inject]
        public SmasherManager(TargetGroupManager targetGroupManager, Smasher smasher)
        {
            this.targetGroupManager = targetGroupManager;
            this.smasher = smasher;
        }

        public void Start()
        {
            targetGroupManager.AddTarget(smasher.transform, CameraTargetGroupTag.Boss());
        }
    }
}