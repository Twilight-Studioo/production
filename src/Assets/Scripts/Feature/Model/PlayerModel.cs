﻿#region

using System;
using Feature.Common.Parameter;
using Feature.Interface;
using Feature.View;
using UniRx;
using UnityEngine;
using VContainer;

#endregion

namespace Feature.Model
{
    public class PlayerModel : IDisposable
    {
        public enum PlayerState
        {
            Idle,
            DoSwap,
        }

        private const float AttackAfterMovableMillis = 250;
        private const int MaxVoltageValue = 100;
        private const int MinVoltageValue = 0;

        private readonly CompositeDisposable attackAfterCompositeDisposable = new();

        private readonly IReactiveProperty<bool> canAttack = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanAttack;

        private readonly IReactiveProperty<bool> canDagger = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanDagger;

        private readonly IReactiveProperty<bool> canEndSwap = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanEndSwap;

        private readonly IReactiveProperty<bool> canStartSwap = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanStartSwap;

        private readonly CharacterParams characterParams;
        private readonly IReactiveProperty<int> daggerStamina;
        public readonly IReadOnlyReactiveProperty<int> DaggerStamina;

        private readonly GameUIView gameUIView;

        private readonly IReactiveProperty<int> health = new ReactiveProperty<int>();

        private readonly CompositeDisposable playerModelTimer = new();

        private readonly IReactiveProperty<PlayerState> playerState;


        private readonly IReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();
        public readonly IReadOnlyReactiveProperty<Vector3> Position;

        public readonly IReadOnlyReactiveProperty<PlayerState> State;

        private readonly IReactiveProperty<int> swapStamina;

        public readonly IReadOnlyReactiveProperty<int> SwapStamina;
        public IReadOnlyReactiveProperty<int> Health;

        private IDisposable recoverStaminaSubscription;

        private IDisposable swapUseStaminaSubscription;
        private IDisposable useDaggerUseStamina;
        public int VoltageValue;

        [Inject]
        public PlayerModel(
            CharacterParams character
            // GameUIView ui
        )
        {
            characterParams = character;
            swapStamina = new ReactiveProperty<int>((int)characterParams.maxHasStamina);
            SwapStamina = swapStamina.ToReadOnlyReactiveProperty();
            daggerStamina = new ReactiveProperty<int>((int)characterParams.maxHasStamina);
            DaggerStamina = daggerStamina.ToReadOnlyReactiveProperty();
            playerState = new ReactiveProperty<PlayerState>(PlayerState.Idle);
            State = playerState.ToReadOnlyReactiveProperty();
            Position = position.ToReadOnlyReactiveProperty();
            CanStartSwap = canStartSwap.ToReadOnlyReactiveProperty();
            CanEndSwap = canEndSwap.ToReadOnlyReactiveProperty();
            CanDagger = canDagger.ToReadOnlyReactiveProperty();
            Health = health.ToReadOnlyReactiveProperty();
            CanAttack = canAttack.ToReadOnlyReactiveProperty();
            // recover stamina


            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    canStartSwap.Value =
                        IfCanStartSwapRate <= (float)swapStamina.Value / characterParams.maxHasStamina;
                })
                .AddTo(playerModelTimer);

            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    canEndSwap.Value = IfCanEndSwapRate <= (float)swapStamina.Value / characterParams.maxHasStamina;
                })
                .AddTo(playerModelTimer);

            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    canDagger.Value =
                        IfCanDaggerRate <= (float)swapStamina.Value / characterParams.maxHasStamina;
                })
                .AddTo(playerModelTimer);

            health.Value = characterParams.health;
            canAttack.Value = true;
        }

        // TODO: スワップに入れるのは、enter+exitスタミナを持っている場合
        public float IfCanStartSwapRate =>
            (float)(characterParams.enterSwapUseStamina + characterParams.swapExecUseStamina) /
            characterParams.maxHasStamina;

        public float IfCanEndSwapRate => (float)characterParams.swapExecUseStamina / characterParams.maxHasStamina;

        private float IfCanDaggerRate => (float)characterParams.useDaggerUseStamina / characterParams.maxHasStamina;
        public float MoveSpeed => characterParams.speed;
        public float JumpForce => characterParams.jumpPower;

        public float JumpMove => characterParams.speed / 2;

        public float ComboTimeWindow => characterParams.comboTimeWindow;
        public float ComboAngleOffset => characterParams.comboAngleOffset;
        public float MaxComboCount => characterParams.maxComboCount;
        public float AttackCoolTime => characterParams.attackCoolTime;

        public Vector3 Forward { get; private set; }

        public void Dispose()
        {
            swapUseStaminaSubscription?.Dispose();
            recoverStaminaSubscription?.Dispose();
            playerModelTimer.Dispose();
            useDaggerUseStamina?.Dispose();
        }

        public event PlayerStateChangeHandler PlayerStateChange;

        public void Start()
        {
            swapStamina.Value = (int)characterParams.maxHasStamina;
        }

        public void OnStartSwap()
        {
            swapStamina.Value = Math.Max(swapStamina.Value - (int)characterParams.enterSwapUseStamina, 0);
            // スワップ中は回復を停止する
            recoverStaminaSubscription?.Dispose();
            swapUseStaminaSubscription?.Dispose();

            swapUseStaminaSubscription = Observable
                .Interval(TimeSpan.FromMilliseconds(characterParams.swapModeStaminaUsageIntervalMillis *
                                                    characterParams.swapContinueTimeScale))
                .Subscribe(_ =>
                {
                    if (playerState.Value == PlayerState.Idle)
                    {
                        return;
                    }

                    SwapUsingUpdate();
                })
                .AddTo(playerModelTimer);
        }


        public void OnEndSwap()
        {
            swapUseStaminaSubscription?.Dispose();
            recoverStaminaSubscription?.Dispose();
            recoverStaminaSubscription = Observable
                .Timer(TimeSpan.FromMilliseconds(characterParams.recoveryTimeMillis))
                .SelectMany(_ =>
                    Observable.Interval(TimeSpan.FromMilliseconds(characterParams.recoveryStaminaTimeMillis)))
                .Subscribe(_ =>
                {
                    if (swapStamina.Value >= characterParams.maxHasStamina || playerState.Value == PlayerState.DoSwap)
                    {
                        return;
                    }

                    swapStamina.Value = Math.Min(swapStamina.Value + (int)characterParams.resourceRecoveryQuantity,
                        (int)characterParams.maxHasStamina);
                })
                .AddTo(playerModelTimer);
        }

        public void OnDagger()
        {
            var daggerUseStamina = (int)characterParams.useDaggerUseStamina;
            if (swapStamina.Value < daggerUseStamina)
            {
                return;
            }

            swapStamina.Value = Math.Max(swapStamina.Value - daggerUseStamina, 0);
        }

        public void ChangeState(PlayerState state)
        {
            playerState.Value = state;
        }

        public void StartSwap()
        {
            if (playerState.Value != PlayerState.Idle || !canStartSwap.Value)
            {
                return;
            }

            PlayerStateChange?.Invoke(PlayerStateEvent.SwapStart);
        }

        public void ExecuteSwap()
        {
            if (canEndSwap.Value && playerState.Value == PlayerState.DoSwap)
            {
                PlayerStateChange?.Invoke(PlayerStateEvent.SwapExec);
                swapStamina.Value = Math.Max(swapStamina.Value - (int)characterParams.swapExecUseStamina, 0);
            }
        }

        public void CancelSwap()
        {
            if (playerState.Value == PlayerState.DoSwap)
            {
                PlayerStateChange?.Invoke(PlayerStateEvent.SwapCancel);
            }
        }

        private void SwapUsingUpdate()
        {
            swapStamina.Value = Math.Max(swapStamina.Value - (int)characterParams.swapModeStaminaUsage, 0);
        }

        public void Attack()
        {
            attackAfterCompositeDisposable.Clear();
            canAttack.Value = false;
            Observable
                .Timer(TimeSpan.FromMilliseconds(AttackAfterMovableMillis))
                .Subscribe(_ => { canAttack.Value = true; })
                .AddTo(attackAfterCompositeDisposable);
        }

        public void UpdatePosition(Vector3 pos)
        {
            // TODO: 要件に合わせて、方向は限定する
            var newPos = new Vector3(pos.x, position.Value.y, position.Value.z);
            Forward = (pos - position.Value).normalized;
            position.Value = pos;
        }

        public void TakeDamage(uint damage)
        {
            health.Value = Mathf.Max((int)(health.Value - damage), 0);
        }

        public void AddVoltageSwap()
        {
            VoltageValue += characterParams.addVoltageSwapValue;
            VoltageValue = Mathf.Clamp(VoltageValue, MinVoltageValue, MaxVoltageValue);
        }

        public int GetVoltageAttackPower()
        {
            if (VoltageValue >= characterParams.useVoltageAttackValue)
            {
                VoltageValue -= characterParams.useVoltageAttackValue;
                return characterParams.attackPower * characterParams.voltageAttackPowerValue;
            }

            return characterParams.attackPower;
        }
    }
}