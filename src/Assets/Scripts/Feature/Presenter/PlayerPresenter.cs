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
        private readonly EnemyParams enemyParams;
        private readonly PlayerModel playerModel;
        private readonly PlayerView playerView;

        private readonly CompositeDisposable swapTimer;

        [Inject]
        public PlayerPresenter(
            PlayerView view,
            PlayerModel model,
            CharacterParams characterParams
        )
        {
            playerView = view;
            playerModel = model;
            this.characterParams = characterParams;
            swapTimer = new();
            playerView.swapRange = characterParams.canSwapDistance;
        }

        public void Start()
        {
            playerView.Position
                .Subscribe(position => { playerModel.UpdatePosition(position); })
                .AddTo(playerView);

            playerView.Health
                .Subscribe(health =>
                {
                    playerModel.TakeDamage(enemyParams.damage);
                    if (health <= 0)
                    {
                        Debug.Log("Player has died.");
                    }
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
    }
}
