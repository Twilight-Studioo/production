using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Feature.Component
{
    public class VolumeController : MonoBehaviour
    {
        //URP関連
        [SerializeField] private Volume volume;

        private ColorAdjustments colorAdjustments; // 追加: ColorAdjustments の参照
        private ColorCurves colorCurves;
        private Vignette vignette;
        private float startIntensity =0.3f;
        [Header("intensityを最大どこまで高くするか")]
        [Range(0.3f,1.0f)]
        [SerializeField]private float endIntensity=5.0f;
        [Header("intensityが戻るまでの時間")]
        public float endvignetteChange = 0.5f;
        private void Awake()
        {
            if (volume != null)
            {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdjustments);
            }
        }

        public void SwapStartUrp(float vignetteChange, float monochrome)
        {
            EnableGrayscale(monochrome);
            GraduallyChangeVignetteColorAndIntensity(Color.black, Color.red, startIntensity, endIntensity,vignetteChange);
        }

        public void SwapFinishUrp()
        {
            DisableGrayscale();
            VignetteBlackColor();
            GraduallyChangeVignetteColorAndIntensity(Color.red, Color.black, endIntensity, startIntensity, endvignetteChange);
            CancelInvoke("VignetteRedColor");
        }

        private void GraduallyChangeVignetteColorAndIntensity(Color startColor, Color endColor, float startIntensity, float endIntensity, float duration)
        {
            if (vignette == null) return;

            float time = 0f;

            Observable.EveryUpdate()
                .TakeWhile(_ => time < duration)
                .Subscribe(_ =>
                    {
                        time += Time.deltaTime;
                        
                        Color newColor = Color.Lerp(startColor, endColor, time / duration);
                        ChangeVignetteColorTo(newColor);
                        
                        float newIntensity = Mathf.Lerp(startIntensity, endIntensity, time / duration);
                        ChangeVignetteIntensityTo(newIntensity);
                    },
                    () =>
                    {
                        ChangeVignetteColorTo(endColor);
                        ChangeVignetteIntensityTo(endIntensity);
                    });
        }
        private void ChangeVignetteIntensityTo(float newIntensity)
        {
            if (vignette != null) vignette.intensity.Override(newIntensity);
        }

        private void VignetteBlackColor()
        {
            if (vignette != null)
                ChangeVignetteColorTo(Color.black);
        }

        // Vignetteの色を変更する
        private void ChangeVignetteColorTo(Color newColor)
        {
            if (vignette != null) vignette.color.Override(newColor);
        }

        // 画面を白黒にする
        private void EnableGrayscale(float monochrome)
        {
            if (colorAdjustments != null) colorAdjustments.saturation.Override(-monochrome);
        }

        // 白黒を解除する
        private void DisableGrayscale()
        {
            if (colorAdjustments != null) colorAdjustments.saturation.Override(0f);
        }
    }
}