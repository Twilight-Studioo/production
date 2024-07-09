#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Feature.View
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private RectTransform fillArea;

        [SerializeField] private RectTransform swapExecLine;
        [SerializeField] private RectTransform swapStartLine;

        private float swapExecValue;
        private float swapStartValue;

        private void Start()
        {
            slider.minValue = 0;
            slider.maxValue = 1;
        }

        private void Update()
        {
            var value = slider.value - swapExecValue;
            var pos = swapExecLine.anchoredPosition;
            pos.x = fillArea.rect.x + (value < 0 ? 0 : value) * fillArea.rect.width;
            swapExecLine.anchoredPosition = pos;
        }

        public void SetVolume(float volume)
        {
            slider.value = volume;
        }

        public void SetExecSwapLine(float value)
        {
            swapExecValue = value;
            var pos = swapExecLine.anchoredPosition;
            pos.x = fillArea.rect.x + value * fillArea.rect.width;
            swapExecLine.anchoredPosition = pos;
        }

        public void SetStartSwapLine(float value)
        {
            swapStartValue = value;
            var pos = swapStartLine.anchoredPosition;
            pos.x = fillArea.rect.x + value * fillArea.rect.width;
            swapStartLine.anchoredPosition = pos;
        }
    }
}