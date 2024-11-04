#region

using System;
using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using Core.Utilities;
using Feature.Interface;
using Main.Controller.GameNavigation;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

#endregion

namespace Main.Controller
{
    /// <summary>
    ///     ゲーム内の入力制御を管理するコントローラークラス
    /// </summary>
    public class InputController : IInputController
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
        public InputController(
            InputActionAccessor accessor,
            ScreenController<Navigation> controller
        )
        {
            this.accessor = accessor.CheckNull();
            this.controller = controller.CheckNull();
        }

        public void Start()
        {
            controller.Hide();
            var pauseActionEvent = accessor.CreateAction(Player.Pause) ??
                                   throw new ArgumentNullException(nameof(Player.Pause));
            pauseActionEvent.Performed += OnPauseReserved;
        }

        private void OnPauseReserved(InputAction.CallbackContext ctx)
        {
            if (controller.CurrentDestination == null)
            {
                controller.Reset();
                controller.Navigate(Navigation.Pause);
                lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                if (controller.CurrentDestination.Route == Navigation.Pause)
                {
                    if (controller.IsShowing)
                    {
                        controller.Hide();
                        Time.timeScale = lastTimeScale;
                    }
                    else
                    {
                        controller.Show();
                        lastTimeScale = Time.timeScale;
                        Time.timeScale = 0;
                    }
                }
            }
        }
    }
}