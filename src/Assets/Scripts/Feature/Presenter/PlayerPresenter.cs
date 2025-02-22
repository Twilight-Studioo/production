#region

using System;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Model;
using Feature.View;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

#endregion

namespace Feature.Presenter
{
    public class PlayerPresenter : IDisposable
    {
        private readonly IAudioMixerController audioMixerController;
        private readonly CharacterParams characterParams;
        private readonly DamageEffectFactory damageEffectFactory;

        private readonly IEndFieldController endFieldController;
        private readonly EnemyParams enemyParams;

        private readonly GameUIView gameUIView;
        private readonly PlayerModel playerModel;

        private readonly CompositeDisposable presenterDisposable = new();

        private readonly CompositeDisposable swapTimer;
        private readonly VoltageBar voltageBar;

        private bool isGameOver;
        private IPlayerView playerView;

        [Inject]
        public PlayerPresenter(
            PlayerModel model,
            CharacterParams characterParams,
            VoltageBar voltageBar,
            GameUIView ui,
            VolumeController volumeController,
            IEndFieldController endFieldController,
            IAudioMixerController audioMixerController,
            DamageEffectFactory damageEffectFactory
        )
        {
            playerModel = model;
            this.characterParams = characterParams;
            gameUIView = ui;
            swapTimer = new();
            this.voltageBar = voltageBar;
            this.endFieldController = endFieldController;
            this.audioMixerController = audioMixerController.CheckNull();
            this.damageEffectFactory = damageEffectFactory.CheckNull();
        }

        public void Dispose()
        {
            presenterDisposable.Dispose();
            swapTimer.Dispose();
        }

        public void OnPossess(IPlayerView view)
        {
            playerView = view;

            playerView.OnDamageEvent += (d, hit) =>
            {
                // pos to hit point
                var rotation = Quaternion.LookRotation(playerView.GetTransform().position - hit);
                damageEffectFactory.PlayEffectAtPosition(playerView.GetTransform().position, rotation,
                    DamageEffectFactory.Type.Player);
                if (playerModel.TakeDamage(d))
                {
                    return new DamageResult.Killed(playerView.GetTransform());
                }

                return new DamageResult.Damaged(playerView.GetTransform());
            };
            playerView.OnHitHandler += OnAttackHitHandler;
            playerView.SwapRange = characterParams.canSwapDistance;
            playerView.GetPositionRef()
                .Subscribe(position =>
                {
                    if (!isGameOver)
                    {
                        playerModel.UpdatePosition(position);
                        //スワップ中ならば一覧を取得してhighlightの処理を呼び出す
                    }
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
                    if (isGameOver)
                    {
                        return;
                    }

                    var playerModelSwapStaminaPer = (float)x / characterParams.maxHasStamina;
                    gameUIView.SetVolume(playerModelSwapStaminaPer);
                })
                .AddTo(presenterDisposable);

            var playerHpBar = Object.FindObjectOfType<PlayerHpBar>();
            playerModel.Health
                .Subscribe(x =>
                {
                    playerHpBar.UpdateHealthBar(x, characterParams.health);
                    if (x <= 0)
                    {
                        isGameOver = true;
                    }
                })
                .AddTo(playerHpBar);
            endFieldController.SubscribeToPlayerHealth(playerModel.Health);
            playerView.SetParam(playerModel.ComboTimeWindow, playerModel.ComboAngleOffset,
                playerModel.MaxComboCount, playerModel.AttackCoolTime, playerModel.MaxComboCoolTime
            );
            playerModel.PlayerStateChange += StateHandler;
        }

        private void OnAttackHitHandler(DamageResult result)
        {
            playerModel.OnEnemyAttacked(result);
            audioMixerController.PlayOneShotSE(AudioAssetType.SlashingHit);
        }

        private void StateHandler(PlayerStateEvent stateEvent)
        {
            if (stateEvent == PlayerStateEvent.SwapStart)
            {
                playerView.IsDrawSwapRange = true;
                swapTimer.Clear();
                Time.timeScale = characterParams.swapContinueTimeScale;
                playerModel.ChangeState(PlayerModel.PlayerState.DoSwap);
                playerModel.OnStartSwap();
                Observable
                    .Timer(TimeSpan.FromMilliseconds(characterParams.swapContinueMaxMillis *
                                                     characterParams.swapContinueTimeScale))
                    .Subscribe(_ => { playerModel.CancelSwap(); })
                    .AddTo(swapTimer);
                playerModel.CanEndSwap
                    .Subscribe(x =>
                    {
                        if (x)
                        {
                            return;
                        }

                        playerModel.CancelSwap();
                    })
                    .AddTo(swapTimer);
            }

            if (stateEvent == PlayerStateEvent.SwapExec)
            {
                EndSwap();
                AddVoltageSwap();
            }

            if (stateEvent == PlayerStateEvent.SwapCancel)
            {
                EndSwap();
            }
        }

        public void Move(float direction)
        {
            if (!isGameOver)
            {
                if (direction > 0)
                {
                    playerView.Move(Vector3.right, playerModel.MoveSpeed);
                }
                else if (direction < 0)
                {
                    playerView.Move(Vector3.left, playerModel.MoveSpeed);
                }
            }
        }

        public void Jump()
        {
            if (!isGameOver)
            {
                playerView.Jump(playerModel.JumpForce);
            }
        }

        public void CancelSwap()
        {
            playerModel.CancelSwap();
        }

        public void ExecuteSwap()
        {
            playerModel.ExecuteSwap();
        }

        public void StartSwap()
        {
            if (playerModel.State.Value != PlayerModel.PlayerState.Idle || !playerModel.CanStartSwap.Value)
            {
                return;
            }

            playerModel.StartSwap();
        }

        private void EndSwap()
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
                    if (t < 1.0f) // If the transition is complete
                    {
                        return;
                    }

                    playerView.IsDrawSwapRange = false;
                    playerModel.ChangeState(PlayerModel.PlayerState.Idle);
                    Time.timeScale = targetTimeScale;
                    swapTimer.Clear();
                })
                .AddTo(swapTimer);
        }

        public void SetPosition(Vector3 position)
        {
            playerView.SetPosition(position);
        }

        public void Attack(float degree)
        {
            if (!isGameOver && playerModel.CanAttack.Value && playerView.CanAttack())
            {
                playerModel.Attack();
                var isSpecialAttack = playerModel.VoltagePower.Value >= characterParams.useVoltageAttackValue;
                playerView.Attack(degree, (uint)playerModel.GetVoltageAttackPower(), isSpecialAttack);
                audioMixerController.PlayOneShotSE(AudioAssetType.Slashing);
                voltageBar.UpdateVoltageBar(playerModel.VoltagePower.Value, characterParams.useVoltageAttackValue,
                    characterParams.votageTwoAttackValue, characterParams.maxVoltage);
                playerView.VoltageEffect(playerModel.VoltagePower.Value, characterParams.useVoltageAttackValue,
                    characterParams.votageTwoAttackValue,
                    characterParams.maxVoltage);
            }
        }

        private void AddVoltageSwap()
        {
            playerModel.AddVoltageSwap();
            voltageBar.UpdateVoltageBar(playerModel.VoltagePower.Value, characterParams.useVoltageAttackValue,
                characterParams.votageTwoAttackValue, characterParams.maxVoltage);
            playerView.VoltageEffect(playerModel.VoltagePower.Value, characterParams.useVoltageAttackValue,
                characterParams.votageTwoAttackValue,
                characterParams.maxVoltage);
        }

        public Transform GetTransform() => playerView.GetTransform();

        public void Dagger(float degree, float h, float v)
        {
            if (!isGameOver)
            {
                playerModel.OnDagger();
                if (playerModel.State.Value != PlayerModel.PlayerState.Idle || !playerModel.CanStartSwap.Value)
                {
                    return;
                }

                playerView.Dagger(degree, h, v);
            }
        }
    }
}