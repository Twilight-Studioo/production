#region

using System;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#endregion

namespace Feature.Component
{
    public class VolumeController : MonoBehaviour
    {
        //URP関連
        [SerializeField] private Volume volume;

        [Header("intensityを最大どこまで高くするか"), Range(0.3f, 1.0f), SerializeField,]
        private float endIntensity = 5.0f;

        [Header("赤になるまでの時間"), SerializeField,] private float vignetteChange = 0.5f;

        [Header("intensityが戻るまでの時間"), SerializeField,]
        private float vignetteChangeDuration = 0.5f;

        [Header("白黒の濃さ"), SerializeField,] private float monochrome = 50;

        private readonly float startIntensity = 0.3f;

        private ColorAdjustments colorAdjustments; // 追加: ColorAdjustments の参照
        private ColorCurves colorCurves;
        private Vignette vignette;

        private IDisposable graduallyDisposable;

        private void Awake()
        {
            if (volume != null)
            {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdjustments);
            }
        }

        public void SwapStartUrp()
        {
            EnableGrayscale(monochrome);
            GraduallyChangeVignetteColorAndIntensity(Color.black, Color.red, startIntensity, endIntensity,
                vignetteChange);
        }

        public void SwapFinishUrp()
        {
            DisableGrayscale();
            VignetteBlackColor();
            GraduallyChangeVignetteColorAndIntensity(Color.red, Color.black, endIntensity, startIntensity,
                vignetteChangeDuration);
        }

        private void GraduallyChangeVignetteColorAndIntensity(Color startColor, Color endColor, float startIntensity,
            float endIntensity, float duration)
        {
            if (vignette == null)
            {
                return;
            }

            var time = 0f;
            graduallyDisposable?.Dispose();
            ChangeVignetteIntensityTo(startIntensity);
            ChangeVignetteColorTo(startColor);
            graduallyDisposable = Observable.EveryUpdate()
                .TakeWhile(_ => time < duration)
                .Subscribe(_ =>
                    {
                        time += Time.deltaTime;

                        var newColor = Color.Lerp(startColor, endColor, time / duration);
                        ChangeVignetteColorTo(newColor);

                        var newIntensity = Mathf.Lerp(startIntensity, endIntensity, time / duration);
                        ChangeVignetteIntensityTo(newIntensity);
                    },
                    () =>
                    {
                        ChangeVignetteColorTo(endColor);
                        ChangeVignetteIntensityTo(endIntensity);
                    })
                .AddTo(this);
        }

        private void ChangeVignetteIntensityTo(float newIntensity)
        {
            if (vignette)
            {
                vignette.intensity.Override(newIntensity);
            }
        }

        private void VignetteBlackColor()
        {
            ChangeVignetteColorTo(Color.black);
        }

        // Vignetteの色を変更する
        private void ChangeVignetteColorTo(Color newColor)
        {
            if (vignette)
            {
                vignette.color.Override(newColor);
            }
        }

        // 画面を白黒にする
        private void EnableGrayscale(float monochrome)
        {
            if (colorAdjustments)
            {
                colorAdjustments.saturation.Override(-monochrome);
            }
        }

        // 白黒を解除する
        private void DisableGrayscale()
        {
            if (colorAdjustments)
            {
                colorAdjustments.saturation.Override(0f);
            }
        }
    }
}