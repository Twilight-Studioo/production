using Core.Input;
using Core.Navigation;
using Core.Utilities;
using Feature.Interface;
using Main.Controller.GameNavigation;
using VContainer;

namespace Main.Controller.SmasherClear
{
    public class SmasherClearController : IOutGameController
    {
        private readonly InputActionAccessor accessor;
        private readonly ScreenController<Navigation> controller;

        [Inject]
        public SmasherClearController(
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
            controller.Navigate(Navigation.SmasherClear);
        }
    }
}