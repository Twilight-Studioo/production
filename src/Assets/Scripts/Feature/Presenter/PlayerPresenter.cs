#region

using System;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Model;
using Feature.View;
using UniRx;
using UnityEditor;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

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

        [Inject]
        public PlayerPresenter(
            PlayerModel model,
            CharacterParams characterParams
        )
        {
            playerModel = model;
            this.characterParams = characterParams;
            swapTimer = new();
        }

        public void OnPossess(PlayerView view)
        {
            playerView = view;

            playerView.OnDamageEvent += playerModel.TakeDamage;
            playerView.SwapRange = characterParams.canSwapDistance;
            playerView.Position
                .Subscribe(position =>
                {
                    playerModel.UpdatePosition(position);
                    //スワップ中ならば一覧を取得してhighlightの処理を呼び出す
                })
                .AddTo(playerView);

            playerModel.Start();

            var playerHpBar = Object.FindObjectOfType<PlayerHPBar>();
            playerModel.Health
                .Subscribe(x =>
                {
                    playerHpBar.UpdateHealthBar(x, characterParams.health);
                    if (x <= 0)
                    {
                        //OnPlayerDeath();
                    }
                })
                .AddTo(playerHpBar);
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
            playerView.Attack(degree, (uint)characterParams.attackPower);
        }

        public void PlayVFX()
        {
            playerView.PlayVFX();
        }

        public Transform GetTransform() => playerView.transform;

        private void OnPlayerDeath()
        {
            Debug.Log("Player has died. Stopping game.");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Dagger(float degree,float h,float v)
        {
            playerModel.OnDagger();
            if (playerModel.State.Value != PlayerModel.PlayerState.Idle || !playerModel.CanStartSwap.Value)
            {
                return;
            }
            playerView.Dagger(degree,h,v);
        }
    }
}