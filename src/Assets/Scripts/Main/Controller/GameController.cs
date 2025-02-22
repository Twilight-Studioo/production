﻿#region

using System;
using Core.Camera;
using Core.Input;
using Core.Input.Generated;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Interface;
using Feature.Model;
using Feature.Presenter;
using Main.Factory;
using UniRx;
using UnityEngine;
using VContainer;

#endregion

namespace Main.Controller
{
    public class GameController : IGameController
    {
        private const float DaggerCooldown = 0.5f;

        private readonly CameraSwitcher cameraSwitcher;
        private readonly CharacterParams characterParams;
        private readonly EnemyFactory enemyFactory;

        private readonly GameSettings gameSettings;

        private readonly InputActionAccessor inputActionAccessor;

        private readonly PlayerModel playerModel;
        private readonly PlayerPresenter playerPresenter;

        private readonly SwapPresenter swapPresenter;

        private readonly TargetGroupManager targetGroupManager;

        private readonly VolumeController volumeController;

        private float horizontalInput;

        private float lastDaggerTime;
        private IDisposable updateDisposable;

        [Inject]
        public GameController(
            PlayerModel playerModel,
            PlayerPresenter playerPresenter,
            SwapPresenter swapPresenter,
            InputActionAccessor inputActionAccessor,
            TargetGroupManager targetGroupManager,
            EnemyFactory enemyFactory,
            GameSettings gameSettings,
            CameraSwitcher cameraSwitcher,
            CharacterParams characterParams,
            VolumeController volumeController
        )
        {
            // DIからの登録
            this.targetGroupManager = targetGroupManager;
            this.playerPresenter = playerPresenter;
            this.inputActionAccessor = inputActionAccessor;
            this.enemyFactory = enemyFactory;
            this.playerModel = playerModel;
            this.swapPresenter = swapPresenter;
            this.gameSettings = gameSettings;
            this.cameraSwitcher = cameraSwitcher;
            this.characterParams = characterParams;
            this.volumeController = volumeController;
        }

        public void Start()
        {
            InputEventSetup();
            Setup();
        }

        public void Dispose()
        {
            playerModel.PlayerStateChange -= StateHandler;
        }

        public void OnPossess(IPlayerView view)
        {
            playerPresenter.OnPossess(view);
            targetGroupManager.SetPlayer(view.GetTransform());
            view.Speed
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x > 0.1)
                    {
                        targetGroupManager.UpdatePlayerForward(view.GetForward(),
                            gameSettings.cameraForwardOffsetFromPlayerMoved);
                    }
                    else
                    {
                        targetGroupManager.UpdatePlayerForward(view.GetForward(), 0f);
                    }
                });
        }

        private void Setup()
        {
            ObjectFactory.Instance.OnObjectCreated += obj =>
            {
                // swapItemがスポーンされたらpresenterに登録
                var item = obj.GetComponent<ISwappable>();
                if (item is not null)
                {
                    swapPresenter.AddItem(item);
                }

                if (obj.TryGetComponent<Dagger>(out var dagger))
                {
                    targetGroupManager.AddTarget(obj.transform, CameraTargetGroupTag.SwapItem());
                    Observable.EveryUpdate()
                        .Select(_ => dagger.transform.position)
                        .DistinctUntilChanged()
                        .Select(_ =>
                            Vector3.Distance(playerPresenter.GetTransform().position, dagger.transform.position) >
                            characterParams.canSwapDistance)
                        .DistinctUntilChanged()
                        .Subscribe(x =>
                        {
                            if (x)
                            {
                                targetGroupManager.RemoveTarget(obj.transform);
                            }
                        })
                        .AddTo(dagger);
                    dagger.OnDestroyEvent += () => targetGroupManager.RemoveTarget(obj.transform);
                }
            };

            enemyFactory.OnAddField += obj =>
            {
                targetGroupManager.AddTarget(obj.GameObject().transform, CameraTargetGroupTag.Enemy());
                var swappable = obj.GameObject().GetComponent<ISwappable>();
                if (swappable != null)
                {
                    swapPresenter.AddItem(swappable);
                }
            };
            enemyFactory.OnRemoveField += obj =>
            {
                targetGroupManager.RemoveTarget(obj.GameObject().transform);
                var swappable = obj.GameObject().GetComponent<ISwappable>();
                if (swappable != null)
                {
                    swapPresenter.RemoveItem(swappable);
                }
            };
            enemyFactory.Subscribe();
        }

        // TODO: この辺りのinput制御を別クラスに切り分ける
        private void InputEventSetup()
        {
            playerModel.PlayerStateChange += StateHandler;
            // Move Setup
            var moveEvent = inputActionAccessor.CreateAction(Player.Move);
            Observable.EveryFixedUpdate()
                .Select(_ => moveEvent.ReadValue<Vector2>())
                .Where(v => v.x != 0f || v.y != 0f)
                .Subscribe(v =>
                {
                    if (playerModel.State.Value == PlayerModel.PlayerState.DoSwap)
                    {
                        swapPresenter.MoveSelector(v, playerModel.Position.Value);
                    }
                    else
                    {
                        // swapPresenter.SelectorStop();

                        if (playerModel.CanAttack.Value)
                        {
                            playerPresenter.Move(v.x);
                        }
                    }
                })
                .AddTo(ObjectFactory.SuperObject);

            var jumpEvent = inputActionAccessor.CreateAction(Player.Jump);
            Observable.EveryFixedUpdate()
                .Select(_ => jumpEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(_ => { playerPresenter.Jump(); })
                .AddTo(ObjectFactory.SuperObject);

            var attackEvent = inputActionAccessor.CreateAction(Player.Attack);
            Observable.EveryFixedUpdate()
                .Select(_ => attackEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        var direction = moveEvent.ReadValue<Vector2>();
                        var degree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        if (degree < 0)
                        {
                            degree += 360;
                        }

                        playerPresenter.Attack(degree);
                    }
                })
                .AddTo(ObjectFactory.SuperObject);

            var daggerEvent = inputActionAccessor.CreateAction(Player.Dagger);
            Observable.EveryFixedUpdate()
                .Select(_ => daggerEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        if (Time.time >= lastDaggerTime + DaggerCooldown)
                        {
                            lastDaggerTime = Time.time;

                            var h = Input.GetAxis("Horizontal");
                            var v = Input.GetAxis("Vertical");
                            var degree = Mathf.Atan2(v, h) * Mathf.Rad2Deg;
                            if (degree < 0)
                            {
                                degree += 360;
                            }

                            playerPresenter.Dagger(degree, h, v);
                        }
                    }
                })
                .AddTo(ObjectFactory.SuperObject);
            var swapEvent = inputActionAccessor.CreateAction(Player.SwapMode);
            Observable.EveryFixedUpdate()
                .Select(_ => swapEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        playerPresenter.StartSwap();
                    }
                    else
                    {
                        if (!playerModel.CanEndSwap.Value || playerModel.State.Value == PlayerModel.PlayerState.Idle ||
                            swapPresenter.SelectItem() == null)
                        {
                            playerPresenter.CancelSwap();
                            return;
                        }

                        playerPresenter.ExecuteSwap();
                    }
                })
                .AddTo(ObjectFactory.SuperObject);
        }

        private void StateHandler(PlayerStateEvent stateEvent)
        {
            updateDisposable?.Dispose();
            if (stateEvent == PlayerStateEvent.SwapStart)
            {
                swapPresenter.InRangeHighlight(playerModel.Position.Value, true);
                updateDisposable = Observable
                    .Interval(TimeSpan.FromMilliseconds(250))
                    .Subscribe(_ =>
                        swapPresenter.InRangeHighlight(playerModel.Position.Value, true)
                    )
                    .AddTo(ObjectFactory.SuperObject);
                cameraSwitcher.UseSwapCamera(true);
                volumeController.SwapStartUrp();
            }

            if (stateEvent == PlayerStateEvent.SwapCancel)
            {
                swapPresenter.SelectorStop();
                swapPresenter.InRangeHighlight(playerModel.Position.Value, false);
                cameraSwitcher.UseSwapCamera(false);
                volumeController.SwapFinishUrp();
            }

            if (stateEvent == PlayerStateEvent.SwapExec)
            {
                var item = swapPresenter.SelectItem();
                swapPresenter.InRangeHighlight(playerModel.Position.Value, false);
                cameraSwitcher.UseSwapCamera(false);
                volumeController.SwapFinishUrp();
                swapPresenter.ResetSelector();
                if (item == null)
                {
                    return;
                }

                item.OnDeselected();
                var pos = playerModel.Position.Value;
                var itemPos = item.GetPosition();
                swapPresenter.Swap(itemPos, pos);
                item.OnSwap(pos);

                playerPresenter.SetPosition(itemPos);
                swapPresenter.SelectorStop();
                item.OnDeselected();
            }
        }
    }
}