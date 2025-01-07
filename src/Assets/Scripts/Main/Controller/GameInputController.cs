#region

using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using Core.Utilities;
using Feature.Interface;
using Main.Controller.GameNavigation;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

#endregion

namespace Main.Controller
{
    /// <summary>
    ///     ゲーム内の入力制御を管理するコントローラークラス
    /// </summary>
    public class GameInputController : IGameInputController
    {
        /// <summary>
        ///     入力アクションへのアクセスを提供するクラス
        /// </summary>
        private readonly InputActionAccessor accessor;

        /// <summary>
        ///     画面遷移を制御するコントローラー
        /// </summary>
        private readonly ScreenController<Navigation> controller;
        
        private float lastTimeScale = 1f;

        [Inject]
        public GameInputController(
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
            controller.Hide();
            var pauseActionEvent = accessor.CreateAction(Player.Pause).CheckNull();
            pauseActionEvent.Performed += OnPauseReserved;

            Observable.EveryUpdate()
                .Select(_ => controller.IsShowing)
                .DistinctUntilChanged()
                .Subscribe(NavigationShowing)
                .AddTo(ObjectFactory.SuperObject);
            NavigationShowing(controller.IsShowing);
        }

        private void OnPauseReserved(InputAction.CallbackContext ctx)
        {
            if (controller.CurrentDestination == null)
            {
                controller.Reset();
                controller.Navigate(Navigation.Pause);
            }
            else if (controller.CurrentDestination.Route == Navigation.Pause)
            {
                if (controller.IsShowing)
                {
                    controller.Hide();
                }
                else
                {
                    controller.Show();
                }
            }
        }
        private void NavigationShowing(bool isShowing)
        {
            if (isShowing)
            {
                lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = lastTimeScale;
            }
        }
    }
}