#region

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

        private readonly GameUIView gameUIView;

        private readonly CharacterParams characterParams;

        private readonly IReactiveProperty<PlayerState> playerState;


        private readonly IReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();

        private readonly CompositeDisposable recoverTimer = new();

        private readonly IReactiveProperty<int> swapResource;

        public readonly IReadOnlyReactiveProperty<int> SwapResource;
        
        public float MoveSpeed => characterParams.speed;
        public float JumpForce => characterParams.jumpPower;

        public float JumpMove => characterParams.speed / 2;

        private readonly IReactiveProperty<bool> continueSwap = new ReactiveProperty<bool>(false);
        public readonly IReadOnlyReactiveProperty<bool> ContinueSwap;

        public IReadOnlyReactiveProperty<PlayerState> State { get; }
        public IReadOnlyReactiveProperty<Vector3> Position { get; private set; }

        [Inject]
        public PlayerModel(
            CharacterParams character,
            GameUIView ui
        )
        {
            gameUIView = ui;
            characterParams = character;
            swapResource = new ReactiveProperty<int>((int)characterParams.maxHasResource);
            SwapResource = swapResource.ToReadOnlyReactiveProperty();
            playerState = new ReactiveProperty<PlayerState>(PlayerState.Idle);
            State = playerState.ToReadOnlyReactiveProperty();
            Position = position.ToReadOnlyReactiveProperty();
            ContinueSwap = continueSwap.ToReadOnlyReactiveProperty();
            // recover resource
            Observable
                .Interval(TimeSpan.FromMilliseconds(characterParams.recoveryResourceTimeMillis))
                .Subscribe(x =>
                {
                    if (swapResource.Value >= characterParams.maxHasResource || playerState.Value == PlayerState.DoSwap)
                    {
                        return;
                    }

                    swapResource.Value = Math.Min(swapResource.Value + (int)characterParams.resourceRecoveryQuantity, (int)characterParams.maxHasResource);
                })
                .AddTo(recoverTimer);

            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    continueSwap.Value = 0 < swapResource.Value - (int)characterParams.swapExecUseResource;
                });
            
            // update ui
            swapResource
                .Subscribe(x =>
                {
                    var volume = (float)x / characterParams.maxHasResource;
                    gameUIView.SetVolume(volume);
                })
                .AddTo(recoverTimer);
        }

        public void Start()
        {
            swapResource.Value = (int)characterParams.maxHasResource;
            var volume = (float)swapResource.Value / characterParams.maxHasResource;
            gameUIView.SetVolume(volume * 100);
        }
        public void Dispose()
        {
            recoverTimer.Dispose();
        }

        public event Action OnAttack;

        public void ChangeState(PlayerState state)
        {
            playerState.Value = state;
        }

        public void Swapped()
        {
            swapResource.Value = Math.Max(swapResource.Value - (int)characterParams.swapExecUseResource, 0);
        }

        public void SwapUsingUpdate()
        {
            swapResource.Value = Math.Max(swapResource.Value - (int)characterParams.swapContinuedUseResource, 0);
        }
        public void Attack()
        {
            OnAttack?.Invoke();
        }

        public void UpdatePosition(Vector3 pos)
        {
            position.Value = pos;
        }
    }
}