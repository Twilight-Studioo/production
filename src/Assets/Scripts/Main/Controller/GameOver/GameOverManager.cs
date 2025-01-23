using Core.Utilities;
using Feature.Interface;
using VContainer;
using VContainer.Unity;

namespace Main.Controller.GameOver
{
    public class GameOverManager : IStartable
    {
        private readonly IOutGameController controller;

        [Inject]
        public GameOverManager(
            IOutGameController controller
        )
        {
            this.controller = controller.CheckNull();
        }

        public void Start()
        {
            controller.Start();
        }
    }
}