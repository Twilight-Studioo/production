using UnityEngine;

namespace Main.Controller.GameNavigation
{
    /// <summary>
    /// コントロール設定画面の動作を制御するクラス
    /// </summary>
    /// <remarks>
    /// AScreenProtocolを継承し、画面のナビゲーションと入力処理を管理します
    /// </remarks>
    public class ControlsScreen: AScreenProtocol
    {
        protected override void OnCancel()
        {
            Controller.PopBackstack();
        }

        protected override void OnNavigation(Vector2 value)
        {
            // No operation
        }

        protected override void OnClick()
        {
            // No operation
        }
    }
}