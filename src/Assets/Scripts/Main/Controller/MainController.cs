#region

using Core.Camera;
using Core.Input;
using Core.Input.Generated;
using Core.Utilities;
using Feature.Common.Parameter;
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
    public class MainController : IGameController
    {
        private readonly EnemyFactory enemyFactory;

        private readonly InputActionAccessor inputActionAccessor;

        private readonly PlayerModel playerModel;
        private readonly PlayerPresenter playerPresenter;

        private readonly SwapPresenter swapPresenter;

        private readonly CameraSwitcher cameraSwitcher;

        private readonly TargetGroupManager targetGroupManager;

        private readonly GameSettings gameSettings;
        private float horizontalInput;

        private float lastDaggerTime; 
        private const float DaggerCooldown = 0.5f; 
        [Inject]
        public MainController(
            PlayerModel playerModel,
            PlayerPresenter playerPresenter,
            SwapPresenter swapPresenter,
            InputActionAccessor inputActionAccessor,
            TargetGroupManager targetGroupManager,
            EnemyFactory enemyFactory,
            GameSettings gameSettings,
            CameraSwitcher cameraSwitcher
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
        }

        public void Start()
        {
            InputEventSetup();
            Setup();
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
                        targetGroupManager.UpdatePlayerForward(view.GetForward(), gameSettings.cameraForwardOffsetFromPlayerMoved);
                    }
                    else
                    {
                        targetGroupManager.UpdatePlayerForward(view.GetForward(), 0f);
                    }
                });
        }

        private void Setup()
        {
            ObjectFactory.OnObjectCreated += obj =>
            {
                // swapItemがスポーンされたらpresenterに登録
                var item = obj.GetComponent<ISwappable>();
                if (item is not null)
                {
                    swapPresenter.AddItem(item);
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
            enemyFactory.OnAddSwappableItem += swapPresenter.AddItem;
            enemyFactory.OnRemoveField += obj =>
            {
                targetGroupManager.RemoveTarget(obj.GameObject().transform);
                var swappable = obj.GameObject().GetComponent<ISwappable>();
                if (swappable != null)
                {
                    swapPresenter.RemoveItem(swappable);
                }
            };
            enemyFactory.GetPlayerTransform = () => playerPresenter.GetTransform();
            enemyFactory.Subscribe();
        }

        // TODO: この辺りのinput制御を別クラスに切り分ける
        private void InputEventSetup()
        {
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
                        swapPresenter.SelectorStop();

                        if (playerModel.CanAttack.Value)
                        {
                            playerPresenter.Move(v.x);
                        }
                    }
                });

            var jumpEvent = inputActionAccessor.CreateAction(Player.Jump);
            Observable.EveryFixedUpdate()
                .Select(_ => jumpEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(_ => { playerPresenter.Jump(); });

            var attackEvent = inputActionAccessor.CreateAction(Player.Attack);
            Observable.EveryFixedUpdate()
                .Select(_ => attackEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        var h = Input.GetAxis("Horizontal");
                        var v = Input.GetAxis("Vertical");
                        var degree = Mathf.Atan2(v, h) * Mathf.Rad2Deg;
                        if (degree < 0)
                        {
                            degree += 360;
                        }

                        playerPresenter.Attack(degree);
                    }
                });

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
                });
            var swapEvent = inputActionAccessor.CreateAction(Player.SwapMode);
            Observable.EveryFixedUpdate()
                .Select(_ => swapEvent.ReadValue<float>() > 0f)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (x)
                    {
                        playerPresenter.StartSwap();
                        swapPresenter.InRangeHighlight(playerModel.Position.Value,true);
                        cameraSwitcher.UseSwapCamera(true);
                    }
                    else
                    {
                        if (!playerModel.CanEndSwap.Value || playerModel.State.Value == PlayerModel.PlayerState.Idle)
                        {
                            swapPresenter.SelectorStop();

                            swapPresenter.InRangeHighlight(playerModel.Position.Value,false);
                            cameraSwitcher.UseSwapCamera(false);
                            return;
                        }

                        var item = swapPresenter.SelectItem();
                        swapPresenter.InRangeHighlight(playerModel.Position.Value,false);
                        cameraSwitcher.UseSwapCamera(false);
                        swapPresenter.ResetSelector();
                        playerPresenter.AddVoltageSwap();
                        playerPresenter.EndSwap();
                        if (item == null)
                        {
                            return;
                        }
                        
                        item.OnDeselected();
                        var pos = playerModel.Position.Value;
                        var itemPos = item.GetPosition();
                        // TODO: 機能をswapPresenterにまとめる
                        swapPresenter.Swap(itemPos, pos);
                        item.OnSwap(pos);

                        playerPresenter.SetPosition(itemPos);
                        swapPresenter.SelectorStop();
                        item.OnDeselected();
                        playerModel.Swapped();
                    }
                });
        }
    }
}