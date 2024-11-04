#region

using System;
using Core.Utilities;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Presenter;
using VContainer;
using VContainer.Unity;

#endregion

namespace Main.Controller
{
    public class GameManager : IStartable, IDisposable
    {
        private readonly IGameController gameController;
        private readonly IInputController inputController;
        private readonly PlayerPresenter playerPresenter;
        private readonly PlayerStart playerStart;

        private bool isStarted;

        [Inject]
        public GameManager(
            PlayerStart playerStart,
            IGameController gameController,
            IInputController inputController
        )
        {
            this.gameController = gameController.CheckNull();
            this.playerStart = playerStart.CheckNull();
            this.inputController = inputController.CheckNull();
        }

        public void Dispose()
        {
            isStarted = false;
        }

        public void Start()
        {
            if (isStarted)
            {
                return;
            }

            isStarted = true;
            var player = playerStart.OnStart();
            if (player == null)
            {
                throw new("Player is not spawned");
            }

            gameController.OnPossess(player);
            gameController.Start();
            inputController.Start();
        }

        public static void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayerStart>();
        }
    }
}