#region

using System;
using Main.Scene.Generated;
using TMPro;
using UniRx;
using UnityEngine;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     ポーズ画面の動作を制御するクラス
    /// </summary>
    public class PauseScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI resumeText;
        [SerializeField] private TextMeshProUGUI goToOptionText;
        [SerializeField] private TextMeshProUGUI goToTitleText;

        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>(Navi.Resume);

        private IDisposable disposable;

        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.Resume;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            resumeText.color = navi == Navi.Resume ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            goToTitleText.color = navi == Navi.GoToTitle ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            goToOptionText.color = navi == Navi.GoToOption ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
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
                    case Navi.GoToOption:
                        navi = Navi.Resume;
                        break;
                    case Navi.GoToTitle:
                        navi = Navi.GoToOption;
                        break;
                    default:
                        return;
                }
            }
            else if (value.y < 0)
            {
                switch (navi)
                {
                    case Navi.Resume:
                        navi = Navi.GoToOption;
                        break;
                    case Navi.GoToOption:
                        navi = Navi.GoToTitle;
                        break;
                    default:
                        return;
                }
            }

            currentNavi.Value = navi;
        }

        protected override void OnClick()
        {
            switch (currentNavi.Value)
            {
                case Navi.Resume:
                    Controller.Hide();
                    break;
                case Navi.GoToOption:
                    Controller.Navigate(Navigation.Option);
                    break;
                case Navi.GoToTitle:
                    Controller.Reset();
                    SceneLoaderFeatures.TitleScene(null).Bind(RootInstance).Load();
                    break;
            }
        }

        private enum Navi
        {
            Resume,
            GoToOption,
            GoToTitle,
        }
    }
}