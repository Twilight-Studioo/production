#region

using System;
using Feature.Presenter;
using VContainer;
using VContainer.Unity;

#endregion

namespace Feature.Common.Environment
{
    public class GameManager : IStartable, IDisposable
    {
        private readonly IGameController gameController;
        private readonly PlayerPresenter playerPresenter;
        private readonly PlayerStart playerStart;

        private bool isStarted;

        [Inject]
        public GameManager(
            PlayerStart playerStart,
            IGameController gameController
        )
        {
            this.gameController = gameController;
            this.playerStart = playerStart;
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
        }

        public static void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayerStart>();
        }
    }
}