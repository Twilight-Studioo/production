#region

using UnityEngine;

#endregion

namespace Core.Utilities.Health
{
    public class FollowObject : MonoBehaviour
    {
        public Transform target; // 追従するターゲットのTransform
        public Vector3 offset = new(0, 2, 0); // オフセット値
        public Canvas canvas; // 使用するCanvas
        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>(); // SliderのRectTransformを取得
        }

        private void Update()
        {
            if (target == null || canvas == null)
            {
                return;
            }

            var worldPosition = target.position + offset; // ターゲットの位置にオフセットを加える
            var screenPosition = Camera.main.WorldToScreenPoint(worldPosition); // ワールド座標をスクリーン座標に変換

            // スクリーン座標をCanvasの座標に変換
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(),
                screenPosition, canvas.worldCamera, out var canvasPosition);

            rectTransform.position = canvasPosition; // Sliderの位置を更新
        }
    }
}