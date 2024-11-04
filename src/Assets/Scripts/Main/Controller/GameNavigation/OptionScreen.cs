using UnityEngine;

namespace Main.Controller.GameNavigation
{
    /// <summary>
    /// オプション画面のUI操作を制御するクラス
    /// </summary>
    public class OptionScreen: AScreenProtocol
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
            Controller.Navigate(Navigation.Volumes);
        }
    }
}