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
        public void SwapStartURP(float vignetteChange)
        {
            EnableGrayscale();
            Invoke("VignetteRedColor", vignetteChange);

        }

        public void SwapFinishURP()
        {
            DisableGrayscale();
            VignetteBlackColor();
            CancelInvoke("VignetteRedColor");
        }

        private void VignetteRedColor()
        {
            if (volume.profile.TryGet(out vignette))
                ChangeVignetteColorTo(Color.red);
        }

        private void VignetteBlackColor()
        {
            if (volume.profile.TryGet(out vignette))
                ChangeVignetteColorTo(Color.black);
        }

        // Vignetteの色を変更する
        private void ChangeVignetteColorTo(Color newColor)
        {
            if (vignette != null) vignette.color.Override(newColor);
        }

        // 画面を白黒にする
        private void EnableGrayscale()
        {
            if (colorAdjustments != null) colorAdjustments.saturation.Override(-55f);
        }

        // 白黒を解除する
        private void DisableGrayscale()
        {
            if (colorAdjustments != null) colorAdjustments.saturation.Override(0f);
        }
    }
}