using Core.Input;
using Core.Navigation;
using Core.Utilities;
using Feature.Interface;
using Main.Controller.GameNavigation;
using VContainer;

namespace Main.Controller.GameOver
{
    public class GameOverController : IOutGameController
    {
        private readonly InputActionAccessor accessor;
        private readonly ScreenController<Navigation> controller;

        [Inject]
        public GameOverController(
            InputActionAccessor accessor,
            ScreenController<Navigation> controller
        )
        {
            this.accessor = accessor.CheckNull();
            this.controller = controller.CheckNull();
        }

        public void Start()
        {
            controller.Reset();
            controller.Navigate(Navigation.GameOver);
        }
    }
}