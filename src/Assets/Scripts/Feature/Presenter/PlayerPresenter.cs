#region

using System;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Interface;
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
    public class PlayerPresenter: IDisposable
    {
        private readonly CharacterParams characterParams;
        private readonly EnemyParams enemyParams;
        private readonly PlayerModel playerModel;

        private readonly CompositeDisposable swapTimer;
        private readonly VoltageBar voltageBar;
        private IPlayerView playerView;

        private readonly GameUIView gameUIView;

        private readonly CompositeDisposable presenterDisposable = new();

        private URP urp;
        [Inject]
        public PlayerPresenter(
            PlayerModel model,
            CharacterParams characterParams,
            VoltageBar voltageBar,
            GameUIView ui,
                URP urp
        )
        {
            playerModel = model;
            this.characterParams = characterParams;
            gameUIView = ui;
            swapTimer = new();
            this.voltageBar = voltageBar;
            this.urp = urp;
        }

        public void OnPossess(IPlayerView view)
        {
            playerView = view;

            playerView.OnDamageEvent += playerModel.TakeDamage;
            playerView.SwapRange = characterParams.canSwapDistance;
            playerView.GetPositionRef()
                .Subscribe(position =>
                {
                    playerModel.UpdatePosition(position);
                    //スワップ中ならば一覧を取得してhighlightの処理を呼び出す
                })
                .AddTo(playerView.GetGameObject());

            playerModel.Start();
            var volume = (float)playerModel.SwapStamina.Value / characterParams.maxHasStamina;
            gameUIView.SetVolume(volume * 100);
            gameUIView.SetExecSwapLine(playerModel.IfCanEndSwapRate);
            gameUIView.SetStartSwapLine(playerModel.IfCanStartSwapRate);
            
            // update ui
            playerModel.SwapStamina
                .Subscribe(x =>
                {
                    var volume = (float)x / characterParams.maxHasStamina;
                    gameUIView.SetVolume(volume);
                })
                .AddTo(presenterDisposable);

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
            playerView.SetParam(playerModel.ComboTimeWindow, playerModel.ComboAngleOffset,playerModel.MaxComboCount,playerModel.VignetteChange,urp,playerModel.Monochrome,playerModel.EndvignetteChange);
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

            playerView.IsDrawSwapRange = true;
            playerView.SwapTimeStartUrp();
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
            playerView.SwapTimeFinishUrp();
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
                        playerView.IsDrawSwapRange = false;
                        playerModel.ChangeState(PlayerModel.PlayerState.Idle);
                        Time.timeScale = targetTimeScale;
                        swapTimer.Clear();
                    }
                })
                .AddTo(swapTimer);
        }

        public void SetPosition(Vector3 position)
        {
            playerView.SetPosition(position);
        }

        public void Attack(float degree)
        {
            playerView.Attack(degree, (uint)playerModel.GetVoltageAttackPower());
            voltageBar.UpdateVoltageBar(playerModel.VoltageValue,characterParams.useVoltageAttackValue);
        }

        //public void PlayVFX()
        //{
        //    playerModel.AddVoltageSwap();
        //    voltageBar.UpdateVoltageBar(playerModel.VoltageValue,characterParams.useVoltageAttackValue);
        //    playerView.PlayVFX();
        //}
        public Transform GetTransform() => playerView.GetTransform();

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

        public void Dispose()
        {
            presenterDisposable.Dispose();
            swapTimer.Dispose();
        }
    }
}