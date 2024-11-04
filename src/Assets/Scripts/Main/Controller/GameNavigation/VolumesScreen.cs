using UnityEngine;

namespace Main.Controller.GameNavigation
{
    /// <summary>
    /// 音量調整画面を制御するクラス
    /// </summary>
    public class VolumesScreen: AScreenProtocol
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
            // TODO: Implement click
        }
    }
}