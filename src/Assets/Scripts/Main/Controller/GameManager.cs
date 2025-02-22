#region

using System;
using Core.Utilities;
using Feature.Common.Constants;
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
        private readonly IAudioMixerController audioMixerController;
        private readonly IGameController gameController;
        private readonly IGameInputController gameInputController;
        private readonly PlayerPresenter playerPresenter;
        private readonly PlayerStart playerStart;

        private bool isStarted;

        [Inject]
        public GameManager(
            PlayerStart playerStart,
            IGameController gameController,
            IGameInputController gameInputController,
            IAudioMixerController audioMixerController
        )
        {
            ObjectFactory.Reset();
            this.gameController = gameController.CheckNull();
            this.playerStart = playerStart.CheckNull();
            this.gameInputController = gameInputController.CheckNull();
            this.audioMixerController = audioMixerController.CheckNull();
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

            audioMixerController.LoadSaveData();

            isStarted = true;
            var player = playerStart.OnStart();
            if (player == null)
            {
                throw new("Player is not spawned");
            }

            gameController.OnPossess(player);
            gameController.Start();
            gameInputController.Start();
            audioMixerController.PlayLoopFile(AudioAssetType.BGM);
        }

        public static void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayerStart>();
        }
    }
}