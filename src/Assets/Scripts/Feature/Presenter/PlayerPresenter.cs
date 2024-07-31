#region

using System;
using Core.Utilities;
using Feature.Common.Parameter;
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
        private readonly EnemyParams enemyParams;
        private readonly PlayerModel playerModel;

        private readonly CompositeDisposable swapTimer;
        private PlayerView playerView;
        private readonly VFXView vfxView;

        [Inject]
        public PlayerPresenter(
            PlayerModel model,
            CharacterParams characterParams,
            VFXView vfxView
        )
        {
            playerModel = model;
            this.characterParams = characterParams;
            swapTimer = new();
            this.vfxView = vfxView;
        }

        public void OnPossess(PlayerView view)
        {
            playerView = view;
            playerView.swapRange = characterParams.canSwapDistance;
            playerView.Position
                .Subscribe(position =>
                {
                    playerModel.UpdatePosition(position);
                    //スワップ中ならば一覧を取得してhighlightの処理を呼び出す
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
                .Timer(TimeSpan.FromMilliseconds(characterParams.swapContinueMaxMillis * characterParams.swapContinueTimeScale))
                .Subscribe(_ => { EndSwap(); })
                .AddTo(swapTimer);
            playerModel.CanEndSwap
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x) { return; }
                    EndSwap();
                });
            
        }

        public void EndSwap()
        {
            if (playerModel.State.Value != PlayerModel.PlayerState.DoSwap)
            {
                return;
            }
            
            swapTimer.Clear();
            playerModel.OnEndSwap();
            
            var swapViews = UnityEngine.Object.FindObjectsOfType<SwapView>();
            foreach (var swap in swapViews)
            {
                swap.PlayVFX();
            }
            vfxView.PlayVFX();
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
                    elapsedTime += Time.unscaledDeltaTime; 
                    var t = Mathf.Clamp01(elapsedTime / duration); 
                    Time.timeScale =
                        Mathf.Lerp(initialTimeScale, targetTimeScale, easingFunction(t)); 
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

        public void Attack(float degree)
        {
            playerView.Attack(degree);
        }

        public Transform GetTransform() => playerView.transform;
    }
}
