using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using Feature.Interface;
using Main.Controller.GameNavigation;
using VContainer;

namespace Main.Controller
{
    public class InputController: IInputController
    {
        private readonly InputActionAccessor accessor;
        private readonly ScreenController<Navigation> controller;

        [Inject]
        public InputController(
            InputActionAccessor accessor,
            ScreenController<Navigation> controller
        )
        {
            this.accessor = accessor;
            this.controller = controller;
        }
        public void Start()
        {
            controller.Hide();
            var pauseActionEvent = accessor.CreateAction(Player.Pause);
            pauseActionEvent.Started += x =>
            {
                if (controller.IsShowing)
                {
                    controller.Hide();
                }
                else
                {
                    controller.Reset();
                    controller.Navigate(Navigation.Pause);
                }
            };
        }
    }
}