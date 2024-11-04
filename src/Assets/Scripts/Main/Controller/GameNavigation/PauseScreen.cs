using Core.Navigation;
using UnityEngine;

namespace Main.Controller.GameNavigation
{
    /// <summary>
    /// ポーズ画面の動作を制御するクラス
    /// </summary>
    public class PauseScreen: AScreenProtocol
    {
        protected override void OnCancel()
        {
            Controller.PopBackstack();
        }

        protected override void OnNavigation(Vector2 value)
        {
            // TODO: Implement navigation
        }

        protected override void OnClick()
        {
            Controller.Navigate(Navigation.Option);
        }
    }
}