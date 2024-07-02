#region

using System;
using Feature.Common;
using Feature.View;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#endregion

namespace Feature.Model
{
    public class PlayerModel : IDisposable, IStartable
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

        public IReadOnlyReactiveProperty<int> SwapResource;

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
            Observable
                .Interval(TimeSpan.FromSeconds(characterParams.secondOfRecoveryResource))
                .Subscribe(x =>
                {
                    if (swapResource.Value >= characterParams.maxHasResource)
                    {
                        return;
                    }

                    swapResource.Value = Math.Min(swapResource.Value + 1, (int)characterParams.maxHasResource);
                    Debug.Log(swapResource);
                })
                .AddTo(recoverTimer);

            swapResource
                .Subscribe(x =>
                {
                    var volume = (float)x / characterParams.maxHasResource;
                    gameUIView.SetVolume(volume * 100);
                })
                .AddTo(recoverTimer);
        }

        public float MoveSpeed => characterParams.speed;
        public float JumpForce => characterParams.jumpPower;

        public float JumpMove => characterParams.speed / 2;

        public bool CanSwap =>
            swapResource.Value - characterParams.swapUsedResource >= 0 && State.Value == PlayerState.Idle;

        public IReadOnlyReactiveProperty<PlayerState> State { get; }
        public IReadOnlyReactiveProperty<Vector3> Position { get; private set; }

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
            swapResource.Value -= (int)characterParams.swapUsedResource;
        }

        public void Attack()
        {
            OnAttack?.Invoke();
        }

        public void UpdatePosition(Vector3 pos)
        {
            Debug.Log(pos);
            position.Value = pos;
        }
    }
}