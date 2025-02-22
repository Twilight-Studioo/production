using System;
using Core.Utilities;
using Feature.Component;
using TMPro;
using UniRx;
using UnityEngine;

namespace Main.Controller.GameNavigation
{
    public class TitleScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI toStartText;
        [SerializeField] private TextMeshProUGUI toContinueText;
        [SerializeField] private TextMeshProUGUI toOptionText;
        [SerializeField] private TextMeshProUGUI toQuitText;
        [SerializeField] private TextMeshProUGUI versionText;

        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>();

        private IDisposable disposable;
        private TitlePlayerAnimation titlePlayerAnimation;

        private void Awake()
        {
            titlePlayerAnimation = FindObjectOfType<TitlePlayerAnimation>();
        }

        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.Start;
            versionText.text = $"version: {Application.version}";
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            toStartText.color = navi == Navi.Start ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            toContinueText.color = navi == Navi.Continue ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            toOptionText.color = navi == Navi.Option ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            toQuitText.color = navi == Navi.Quit ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
        }

        protected override void OnCancel()
        {
            currentNavi.Value = Navi.Quit;
        }

        protected override void OnNavigation(Vector2 value)
        {
            var navi = currentNavi.Value;
            if (value.y > 0)
            {
                switch (navi)
                {
                    case Navi.Continue:
                        navi = Navi.Start;
                        break;
                    //case Navi.Option:
                    //    navi = Navi.Continue;
                    //    break;
                    case Navi.Option:
                        navi = Navi.Start;
                        break;
                    case Navi.Quit:
                        navi = Navi.Option;
                        break;
                }
            }
            else if (value.y < 0)
            {
                switch (navi)
                {
                    //case Navi.Start:
                    //    navi = Navi.Continue;
                    //    break;
                    case Navi.Start:
                        navi = Navi.Option;
                        break;
                    case Navi.Continue:
                        navi = Navi.Option;
                        break;
                    case Navi.Option:
                        navi = Navi.Quit;
                        break;
                }
            }

            currentNavi.Value = navi;
        }

        protected override void OnClick()
        {
            switch (currentNavi.Value)
            {
                case Navi.Start:
                    // TODO: production code here
                    Controller.Navigate(Navigation.StageSelect);
                    titlePlayerAnimation.OnClickStage();
                    break;
                case Navi.Continue:
                    // TODO: Continue the game logic here
                    Controller.Navigate(Navigation.StageSelect);
                    titlePlayerAnimation.OnClickStage();
                    break;
                case Navi.Option:
                    Controller.Navigate(Navigation.Option);
                    break;
                case Navi.Quit:
                    SceneUtility.Finished();
                    break;
            }
        }

        private enum Navi
        {
            Start,
            Continue,
            Option,
            Quit,
        }
    }
}