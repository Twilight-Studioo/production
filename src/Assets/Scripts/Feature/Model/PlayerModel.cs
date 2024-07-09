﻿#region

using System;
using Feature.Common;
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

        private readonly IReactiveProperty<bool> canEndSwap = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanEndSwap;

        private readonly IReactiveProperty<bool> canStartSwap = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> CanStartSwap;

        private readonly CharacterParams characterParams;

        private readonly GameUIView gameUIView;

        private readonly CompositeDisposable playerModelTimer = new();

        private readonly IReactiveProperty<PlayerState> playerState;


        private readonly IReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();

        private readonly IReactiveProperty<int> swapStamina;

        public readonly IReadOnlyReactiveProperty<int> SwapStamina;

        private IDisposable recoverStaminaSubscription;

        private IDisposable swapUseStaminaSubscription;

        [Inject]
        public PlayerModel(
            CharacterParams character,
            GameUIView ui
        )
        {
            gameUIView = ui;
            characterParams = character;
            swapStamina = new ReactiveProperty<int>((int)characterParams.maxHasStamina);
            SwapStamina = swapStamina.ToReadOnlyReactiveProperty();
            playerState = new ReactiveProperty<PlayerState>(PlayerState.Idle);
            State = playerState.ToReadOnlyReactiveProperty();
            Position = position.ToReadOnlyReactiveProperty();
            CanStartSwap = canStartSwap.ToReadOnlyReactiveProperty();
            CanEndSwap = canEndSwap.ToReadOnlyReactiveProperty();
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

            // update ui
            swapStamina
                .Subscribe(x =>
                {
                    var volume = (float)x / characterParams.maxHasStamina;
                    gameUIView.SetVolume(volume);
                })
                .AddTo(playerModelTimer);
        }

        // TODO: スワップに入れるのは、enter+exitスタミナを持っている場合
        private float IfCanStartSwapRate =>
            (float)(characterParams.enterSwapUseStamina + characterParams.swapExecUseStamina) /
            characterParams.maxHasStamina;

        private float IfCanEndSwapRate => (float)characterParams.swapExecUseStamina / characterParams.maxHasStamina;

        public float MoveSpeed => characterParams.speed;
        public float JumpForce => characterParams.jumpPower;

        public float JumpMove => characterParams.speed / 2;

        public IReadOnlyReactiveProperty<PlayerState> State { get; }
        public IReadOnlyReactiveProperty<Vector3> Position { get; private set; }

        public Vector3 Forward { get; private set; }

        public void Dispose()
        {
            swapUseStaminaSubscription?.Dispose();
            recoverStaminaSubscription?.Dispose();
            playerModelTimer.Dispose();
        }

        public void Start()
        {
            swapStamina.Value = (int)characterParams.maxHasStamina;
            var volume = (float)swapStamina.Value / characterParams.maxHasStamina;
            gameUIView.SetVolume(volume * 100);
            gameUIView.SetExecSwapLine(IfCanEndSwapRate);
            gameUIView.SetStartSwapLine(IfCanStartSwapRate);
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

        public event Action OnAttack;

        public void ChangeState(PlayerState state)
        {
            playerState.Value = state;
        }

        public void Swapped()
        {
            swapStamina.Value = Math.Max(swapStamina.Value - (int)characterParams.swapExecUseStamina, 0);
        }

        private void SwapUsingUpdate()
        {
            swapStamina.Value = Math.Max(swapStamina.Value - (int)characterParams.swapModeStaminaUsage, 0);
        }

        public void Attack()
        {
            OnAttack?.Invoke();
        }

        public void UpdatePosition(Vector3 pos)
        {
            // TODO: 要件に合わせて、方向は限定する
            Forward = (pos - position.Value).normalized;
            position.Value = pos;
        }
    }
}