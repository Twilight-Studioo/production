#region

using System;
using Main.Scene.Generated;
using TMPro;
using UniRx;
using UnityEngine;

#endregion

namespace Main.Controller.GameNavigation
{
    public class ClearSmasherScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI goToRestart;
        [SerializeField] private TextMeshProUGUI goToTitleText;

        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>(Navi.GoToRestart);

        private IDisposable disposable;

        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.GoToRestart;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            goToRestart.color = navi == Navi.GoToRestart ? Color.white : Color.gray;
            goToTitleText.color = navi == Navi.GoToTitle ? Color.white : Color.gray;
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
                    case Navi.GoToRestart:
                        navi = Navi.GoToTitle;
                        break;
                    case Navi.GoToTitle:
                        navi = Navi.GoToRestart;
                        break;
                    default:
                        return;
                }
            }
            else if (value.y < 0)
            {
                switch (navi)
                {
                    case Navi.GoToRestart:
                        navi = Navi.GoToTitle;
                        break;
                    case Navi.GoToTitle:
                        navi = Navi.GoToRestart;
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
                case Navi.GoToRestart:
                    Controller.Reset();
                    // SceneLoaderFeatures.SmasherBossFight(null).Bind(RootInstance).Load();
                    SceneLoaderFeatures.boss(null).Bind(RootInstance).Load();
                    break;
                case Navi.GoToTitle:
                    Controller.Reset();
                    SceneLoaderFeatures.TitleScene(null).Bind(RootInstance).Load();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum Navi
        {
            GoToRestart,
            GoToTitle,
        }
    }
}