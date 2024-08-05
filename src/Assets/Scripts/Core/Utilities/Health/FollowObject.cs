using UnityEngine;

namespace Core.Utilities.Health
{
    public class FollowObject : MonoBehaviour
    {
        public Transform target; // 追従するターゲットのTransform
        public Vector3 offset = new Vector3(0, 2, 0); // オフセット値
        public Canvas canvas; // 使用するCanvas
        private RectTransform rectTransform;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>(); // SliderのRectTransformを取得
        }

        void Update()
        {
            if (target == null || canvas == null) return;

            Vector3 worldPosition = target.position + offset; // ターゲットの位置にオフセットを加える
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition); // ワールド座標をスクリーン座標に変換
            Vector3 canvasPosition;

            // スクリーン座標をCanvasの座標に変換
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out canvasPosition);

            rectTransform.position = canvasPosition; // Sliderの位置を更新
        }
    }
}