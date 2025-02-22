#region

using System;
using System.Collections;
using Feature.Component;
using Main.Scene.Generated;
using Main.Scene.Model;
using TMPro;
using UniRx;
using UnityEngine;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     ステージ選択画面の動作を制御するクラス
    /// </summary>
    public class StageSelectScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI AText;
        [SerializeField] private TextMeshProUGUI BText;
        [SerializeField] private TextMeshProUGUI backText;
        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>(Navi.A);

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
            currentNavi.Value = Navi.A;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            AText.color = navi == Navi.A ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
            BText.color = navi == Navi.B ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
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
                    case Navi.B:
                        navi = Navi.A;
                        break;
                    case Navi.Back:
                        navi = Navi.B;
                        break;
                }
            }
            else if (value.y < 0)
            {
                switch (navi)
                {
                    case Navi.A:
                        navi = Navi.B;
                        break;
                    case Navi.B:
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
                case Navi.A:
                    titlePlayerAnimation.StageSelect();
                    StartCoroutine(LoadStageWithDelay(SceneLoaderFeatures.zako(new ZakoSceneDataModel())));
                    break;
                case Navi.B:
                    titlePlayerAnimation.StageSelect();
                    StartCoroutine(LoadStageWithDelay(SceneLoaderFeatures.boss(new BossSceneDataModel())));
                    break;
                case Navi.Back:
                    titlePlayerAnimation.ClickBacktoTitle();
                    Controller.PopBackstack();
                    break;
            }
        }

        private IEnumerator LoadStageWithDelay(SceneLoader sceneLoader)
        {
            yield return new WaitForSeconds(2.0f);
            Controller.Reset();
            sceneLoader.Bind(RootInstance).Load();
        }

        private enum Navi
        {
            A,
            B,
            Back,
        }
    }
}