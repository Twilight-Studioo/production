#region

using Core.Camera;
using Core.Input;
using Core.Input.Generated;
using Feature.Model;
using Feature.Presenter;
using Feature.View;
using Main.Environment;
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

        private readonly TargetGroupManager targetGroupManager;
        private float horizontalInput;

        private float lastDaggerTime; 
        private const float daggerCooldown = 0.5f; 
        [Inject]
        public MainController(
            PlayerModel playerModel,
            PlayerPresenter playerPresenter,
            SwapPresenter swapPresenter,
            InputActionAccessor inputActionAccessor,
            TargetGroupManager targetGroupManager,
            EnemyFactory enemyFactory
        )
        {
            // DIからの登録
            this.targetGroupManager = targetGroupManager;
            this.playerPresenter = playerPresenter;
            this.inputActionAccessor = inputActionAccessor;
            this.enemyFactory = enemyFactory;
            this.playerModel = playerModel;
            this.swapPresenter = swapPresenter;
        }

        public void Start()
        {
            InputEventSetup();
            Setup();
        }

        public void OnPossess(PlayerView view)
        {
            playerPresenter.OnPossess(view);
            targetGroupManager.AddTarget(view.transform, CameraTargetGroupTag.Player());
        }

        private void Setup()
        {
            enemyFactory.OnAddField += obj =>
            {
                targetGroupManager.AddTarget(obj.transform, CameraTargetGroupTag.Enemy());
            };
            enemyFactory.OnRemoveField += obj =>
            {
                targetGroupManager.RemoveTarget(obj.transform);
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
                        playerPresenter.Move(v.x);
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
                        if (Time.time >= lastDaggerTime + daggerCooldown)
                        {
                            lastDaggerTime = Time.time; 
                            
                            var h = Input.GetAxis("Horizontal");
                            var v = Input.GetAxis("Vertical");
                            float degree = Mathf.Atan2(v, h) * Mathf.Rad2Deg;
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
                    }
                    else
                    {
                        if (!playerModel.CanEndSwap.Value || playerModel.State.Value == PlayerModel.PlayerState.Idle)
                        {
                            return;
                        }

                        var item = swapPresenter.SelectItem();

                        swapPresenter.ResetSelector();
                        playerPresenter.EndSwap();
                        if (item == null)
                        {
                            return;
                        }

                        item.PlayVFX();
                        playerPresenter.PlayVFX();

                        var pos = playerModel.Position.Value;
                        playerPresenter.SetPosition(item.transform.position);
                        item.SetPosition(pos);
                        item.SetHighlight(false);
                        playerModel.Swapped();
                    }
                });
        }
    }
}