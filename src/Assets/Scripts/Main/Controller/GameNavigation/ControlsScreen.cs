#region

using System;
using TMPro;
using UniRx;
using UnityEngine;

#endregion

namespace Main.Controller.GameNavigation
{
    /// <summary>
    ///     コントロール設定画面の動作を制御するクラス
    /// </summary>
    /// <remarks>
    ///     AScreenProtocolを継承し、画面のナビゲーションと入力処理を管理します
    /// </remarks>
    public class ControlsScreen : ScreenProtocol<Navigation>
    {
        [SerializeField] private TextMeshProUGUI backText;

        private readonly IReactiveProperty<Navi> currentNavi = new ReactiveProperty<Navi>();

        private IDisposable disposable;

        public override void OnShow()
        {
            base.OnShow();
            disposable = currentNavi.Subscribe(OnNaviChanged);
            currentNavi.Value = Navi.Back;
        }

        public override void OnHide()
        {
            base.OnHide();
            disposable?.Dispose();
        }

        private void OnNaviChanged(Navi navi)
        {
            backText.color = navi == Navi.Back ? Color.white : Color.HSVToRGB(0.6f, 0.2f, 0.6f);
        }


        protected override void OnCancel()
        {
            Controller.PopBackstack();
        }

        protected override void OnNavigation(Vector2 value)
        {
            // No operation
        }

        protected override void OnClick()
        {
            switch (currentNavi.Value)
            {
                case Navi.Back:
                    Controller.PopBackstack();
                    break;
            }
        }

        private enum Navi
        {
            Back,
        }
    }
}