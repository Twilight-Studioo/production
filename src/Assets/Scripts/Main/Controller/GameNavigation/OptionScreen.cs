#region

using System;
using TMPro;
using UniRx;
using UnityEngine;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     オプション画面のUI操作を制御するクラス
    /// </summary>
    public class OptionScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI volumesText;
        [SerializeField] private TextMeshProUGUI controlsText;
        [SerializeField] private TextMeshProUGUI backText;

        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>();

        private IDisposable disposable;
        
        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.Volumes;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            volumesText.color = navi == Navi.Volumes ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            controlsText.color = navi == Navi.Controls ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            backText.color = navi == Navi.Back ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
        }
        
        protected override void OnCancel()
        {
            Controller.PopBackstack();
        }

        protected override void OnNavigation(Vector2 value)
        {
            var navi = currentNavi.Value;
            if (value.y > 0)
            {
                switch (navi)
                {
                    case Navi.Controls:
                        navi = Navi.Volumes;
                        break;
                    case Navi.Back:
                        navi = Navi.Controls;
                        break;
                }
            }
            else if (value.y < 0)
            {
                switch (navi)
                {
                    case Navi.Volumes:
                        navi = Navi.Controls;
                        break;
                    case Navi.Controls:
                        navi = Navi.Back;
                        break;
                }
            }
            
            currentNavi.Value = navi;
        }

        protected override void OnClick()
        {
            switch (currentNavi.Value)
            {
                case Navi.Volumes:
                    Controller.Navigate(Navigation.Volumes);
                    break;
                case Navi.Controls:
                    Controller.Navigate(Navigation.Controls);
                    break;
                case Navi.Back:
                    Controller.PopBackstack();
                    break;
            }
        }
        
        private enum Navi
        {
            Volumes,
            Controls,
            Back,
        }
    }
}