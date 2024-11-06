#region

using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     音量調整画面を制御するクラス
    /// </summary>
    public class VolumesScreen : ScreenProtocol
    {
        
        [SerializeField] private TextMeshProUGUI masterSliderText;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private TextMeshProUGUI bgmSliderText;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private TextMeshProUGUI seSliderText;
        [SerializeField] private Slider seSlider;
        [SerializeField] private TextMeshProUGUI backText;

        
        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>();

        private IDisposable disposable;
        
        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.Master;
            masterSlider.maxValue = 1;
            masterSlider.minValue = 0;
            bgmSlider.maxValue = 1;
            bgmSlider.minValue = 0;
            seSlider.maxValue = 1;
            seSlider.minValue = 0;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            masterSliderText.color = navi == Navi.Master ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            bgmSliderText.color = navi == Navi.BGM ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            seSliderText.color = navi == Navi.Se ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            backText.color = navi == Navi.Back ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
        }
        protected override void OnCancel()
        {
            Controller.PopBackstack();
        }

        protected override void OnNavigation(Vector2 value)
        {
            if (value.y != 0)
            {
                OnNavigationVertical(value.y);
            }
            else if (value.x != 0)
            {
                OnNavigationHorizontal(value.x);
            }
        }
        
        protected void OnNavigationHorizontal(float value)
        {
            var changed = value > 0 ? 0.1f : -0.1f;
            switch (currentNavi.Value)
            {
                case Navi.Master:
                    masterSlider.value += changed;
                    break;
                case Navi.BGM:
                    bgmSlider.value += changed;
                    break;
                case Navi.Se:
                    seSlider.value += changed;
                    break;
            }
        }
        
        private void OnNavigationVertical(float value)
        {
            if (value > 0)
            {
                switch (currentNavi.Value)
                {
                    case Navi.BGM:
                        currentNavi.Value = Navi.Master;
                        break;
                    case Navi.Se:
                        currentNavi.Value = Navi.BGM;
                        break;
                    case Navi.Back:
                        currentNavi.Value = Navi.Se;
                        break;
                }
            }
            else if (value < 0)
            {
                switch (currentNavi.Value)
                {
                    case Navi.Master:
                        currentNavi.Value = Navi.BGM;
                        break;
                    case Navi.BGM:
                        currentNavi.Value = Navi.Se;
                        break;
                    case Navi.Se:
                        currentNavi.Value = Navi.Back;
                        break;
                }
            }
        }

        protected override void OnClick()
        {
            switch (currentNavi.Value)
            {
                case Navi.Master:
                    break;
                case Navi.BGM:
                    break;
                case Navi.Se:
                    break;
                case Navi.Back:
                    Controller.PopBackstack();
                    break;
            }
        }
        
        private enum Navi
        {
            Master,
            BGM,
            Se,
            Back,
        }
    }
}