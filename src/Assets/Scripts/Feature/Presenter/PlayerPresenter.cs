#region

using System;
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
        }

        public void Start()
        {
            playerView.Position
                .Subscribe(position =>
                {
                    playerModel.UpdatePosition(position);
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
            if (playerModel.State.Value != PlayerModel.PlayerState.Idle)
            {
                return;
            }

            playerModel.ChangeState(PlayerModel.PlayerState.DoSwap);
            Time.timeScale = characterParams.swapContinueTimeScale;
            swapTimer.Clear();
            Observable
                .Timer(TimeSpan.FromMilliseconds(characterParams.swapContinueMaxMillis * Time.timeScale))
                .Subscribe(_ =>
                {
                    EndSwap();
                    swapTimer.Clear();
                })
                .AddTo(swapTimer);
            playerModel.ContinueSwap
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        return;
                    }

                    EndSwap();
                    swapTimer.Clear();
                });
    
            // on used
            Observable
                .Interval(TimeSpan.FromMilliseconds(characterParams.swapContinueUsageTimeMillis * Time.timeScale))
                .Subscribe(_ =>
                {
                    if (playerModel.State.Value == PlayerModel.PlayerState.Idle)
                    {
                        return;
                    }
                    playerModel.SwapUsingUpdate();
                })
                .AddTo(swapTimer);
        }

        public void EndSwap()
        {
            Time.timeScale = 1f;
            playerModel.ChangeState(PlayerModel.PlayerState.Idle);
        }

        public void SetPosition(Vector3 position)
        {
            playerView.transform.position = position;
        }

        public void Attack()
        {
            playerModel.Attack(); // モデルの攻撃メソッドを呼び出し、ビューの攻撃アニメーションをトリガーする
        }
    }
}