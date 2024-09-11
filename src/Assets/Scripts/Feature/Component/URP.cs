using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Feature.Component
{
    public class URP : MonoBehaviour
    {
        //URP関連
        [SerializeField] private Volume volume;

        private ColorAdjustments colorAdjustments; // 追加: ColorAdjustments の参照
        private ColorCurves colorCurves;
        private Vignette vignette;
        private float startIntensity =0.3f;
        private float endIntensity=0.6f;

        private void Awake()
        {
            if (volume != null)
            {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdjustments);
            }
        }

        public void SwapStartURP(float vignetteChange, float monochrome)
        {
            EnableGrayscale(monochrome);
            GraduallyChangeVignetteColorAndIntensity(Color.black, Color.red, vignetteChange, startIntensity, endIntensity);
        }

        public void SwapFinishURP()
        {
            DisableGrayscale();
            VignetteBlackColor();
            GraduallyChangeVignetteColorAndIntensity(Color.red, Color.black, 1f, 0f, 0.3f); // intensityを1から0へ
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

                        // 色の補間
                        Color newColor = Color.Lerp(startColor, endColor, time / duration);
                        ChangeVignetteColorTo(newColor);

                        // intensityの補間
                        float newIntensity = Mathf.Lerp(startIntensity, endIntensity, time / duration);
                        ChangeVignetteIntensityTo(newIntensity);
                    },
                    () =>
                    {
                        // 最後に完全な色とintensityを設定
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