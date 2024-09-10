using System;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Feature.Component
{
    public class URP : MonoBehaviour
    {
        //URP関連
        [SerializeField]private Volume volume;

        private ColorAdjustments colorAdjustments; // 追加: ColorAdjustments の参照
        private ColorCurves colorCurves;
        private Vignette vignette;

        private void Awake()
        {
            if (volume != null)
            {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdjustments);
            }
        }
        public void SwapStartURP(float vignetteChange,float monochrome)
        {
            EnableGrayscale(monochrome);
            GraduallyChangeVignetteColor(Color.black, Color.red, vignetteChange);

        }

        public void SwapFinishURP()
        {
            DisableGrayscale();
            VignetteBlackColor();
            CancelInvoke("VignetteRedColor");
        }

        private void GraduallyChangeVignetteColor(Color startColor, Color endColor, float vignetteChange)
        {
            if (vignette == null) return;

            float time = 0f;
    
            // 濃い赤色にするために RGB を調整 (例えば 0.8 の赤)
            Color deepRed = new Color(0.95f, 0.0f, 0.0f); 

            Observable.EveryUpdate()
                .TakeWhile(_ => time < vignetteChange)
                .Subscribe(_ =>
                    {
                        time += Time.deltaTime;
                        Color newColor = Color.Lerp(startColor, deepRed, time / vignetteChange);
                        ChangeVignetteColorTo(newColor);
                    },
                    () => ChangeVignetteColorTo(deepRed)); // 最後に濃い赤色に設定
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