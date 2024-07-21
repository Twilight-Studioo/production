#region

using System;
using Core.Utilities;
using Feature.Common;
using Feature.Model;
using Feature.View;
using UniRx;
using UnityEngine;
using VContainer;

#endregion

namespace Feature.Presenter
{
    public class PlayerPresenter
    {
        private readonly CharacterParams characterParams;
        private readonly PlayerModel playerModel;
        private readonly PlayerView playerView;

        private readonly CompositeDisposable swapTimer;

        private readonly VFXView vfxView;

        [Inject]
        public PlayerPresenter(
            PlayerView view,
            PlayerModel model,
            CharacterParams characterParams,
            VFXView vfxView
        )
        {
            playerView = view;
            playerModel = model;
            this.characterParams = characterParams;
            swapTimer = new();
            playerView.swapRange = characterParams.canSwapDistance;
            this.vfxView = vfxView;
        }

        public void Start()
        {
            playerView.Position
                .Subscribe(position =>
                {
                    playerModel.UpdatePosition(position);
                    //スワップ中ならば一覧を取得してhilightの処理を呼び出す
                })
                .AddTo(playerView);
            playerModel.Start();
        }

        public void Move(float direction)
        {
            playerView.Move(direction * playerModel.MoveSpeed, playerModel.JumpMove);
        }

        public void Jump()
        {
            playerView.Jump(playerModel.JumpForce);
        }


        public void StartSwap()
        {
            if (playerModel.State.Value != PlayerModel.PlayerState.Idle || !playerModel.CanStartSwap.Value)
            {
                return;
            }

            playerView.isDrawSwapRange = true;
            swapTimer.Clear();
            Time.timeScale = characterParams.swapContinueTimeScale;
            playerModel.ChangeState(PlayerModel.PlayerState.DoSwap);
            playerModel.OnStartSwap();
            Observable
                .Timer(TimeSpan.FromMilliseconds(characterParams.swapContinueMaxMillis *
                                                 characterParams.swapContinueTimeScale))
                .Subscribe(_ => { EndSwap(); })
                .AddTo(swapTimer);
            playerModel.CanEndSwap
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        return;
                    }
                    EndSwap();
                });
        }

        public void EndSwap()
        {
            if (playerModel.State.Value != PlayerModel.PlayerState.DoSwap)
            {
                return;
            }
            vfxView.PlayVFX();
            swapTimer.Clear();
            playerModel.OnEndSwap();

            Func<float, float> easingFunction;

            
            
            switch (characterParams.swapReturnCurve)
            {
                case SwapReturnCurve.EaseIn:
                    easingFunction = Easing.EaseIn;
                    break;
                case SwapReturnCurve.EaseOut:
                    easingFunction = Easing.EaseOut;
                    break;
                case SwapReturnCurve.EaseInOut:
                    easingFunction = Easing.EaseInOut;
                    break;
                case SwapReturnCurve.Linear:
                default:
                    easingFunction = Easing.Linear;
                    break;
            }

            var initialTimeScale = characterParams.swapContinueTimeScale;
            const float targetTimeScale = 1.0f;
            var duration = characterParams.swapReturnTimeMillis / 1000f;
            var elapsedTime = 0f;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    elapsedTime += Time.unscaledDeltaTime; // Update elapsed time with unscaled time
                    var t = Mathf.Clamp01(elapsedTime / duration); // Calculate normalized time
                    Time.timeScale =
                        Mathf.Lerp(initialTimeScale, targetTimeScale, easingFunction(t)); // Apply easing function
                    if (t >= 1.0f) // If the transition is complete
                    {
                        playerView.isDrawSwapRange = false;
                        playerModel.ChangeState(PlayerModel.PlayerState.Idle);
                        Time.timeScale = targetTimeScale;
                        swapTimer.Clear();
                    }
                })
                .AddTo(swapTimer);
        }

        public void SetPosition(Vector3 position)
        {
            playerView.transform.position = position;
        }

        public void Attack()
        {
            var direction = playerModel.Forward;
            playerView.Attack(direction); // モデルの攻撃メソッドを呼び出し、ビューの攻撃アニメーションをトリガーする
        }
    }
}