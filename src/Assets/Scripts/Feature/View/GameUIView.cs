#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Feature.View
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private void Start()
        {
            slider.minValue = 0;
            slider.maxValue = 1;
        }

        public void SetVolume(float volume)
        {
            slider.value = volume;
        }
    }
}