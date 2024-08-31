using System;
using Feature.View;
using Main.Environment;
using Moq;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace Editor.Tests.Main
{
    public class InjectionEditModeTests
    {
        private GameManager gameManager;
        private Mock<IGameController> mockGameController;
        private PlayerStart playerStart;
        private GameObject playerPrefab;
        private GameObject playerGameObject;
        private PlayerView playerView;

        [SetUp]
        public void SetUp()
        {
            // 依存関係のモックを作成
            mockGameController = new Mock<IGameController>();

            // テスト用のGameObjectとPlayerStartコンポーネントを作成
            var playerStartGameObject = new GameObject();
            playerStart = playerStartGameObject.AddComponent<PlayerStart>();

            // テスト用のプレイヤーのPrefabとそのPlayerViewコンポーネントを作成
            playerGameObject = new GameObject("Player");
            playerView = playerGameObject.AddComponent<PlayerView>();
            playerPrefab = playerGameObject;

            // PlayerStartにプレイヤープレハブを設定
            playerStart
                .GetType()
                .GetField("playerRef", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(playerStart, playerPrefab);

            // GameManagerのインスタンスを作成
            gameManager = new GameManager(playerStart, mockGameController.Object);
        }

        [Test]
        public void Start_ShouldNotStartIfAlreadyStarted()
        {
            // Arrange
            gameManager.Start();

            // Act
            gameManager.Start();

            // Assert
            // 2回目のStartではプレイヤーが生成されないこと
            Assert.IsTrue(playerStart.OnStart() == null);
            mockGameController.Verify(x => x.OnPossess(It.IsAny<PlayerView>()), Times.Once);
            mockGameController.Verify(x => x.Start(), Times.Once);
        }

        [Test]
        public void Start_ShouldThrowExceptionIfPlayerNotSpawned()
        {
            // Arrange
            playerStart
                .GetType()
                .GetField("isPlayerSpawned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(playerStart, true);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => gameManager.Start());
            Assert.AreEqual("Player is not spawned", ex.Message);
        }

        [Test]
        public void Start_ShouldCallGameControllerMethodsIfPlayerSpawned()
        {
            // Arrange
            playerStart
                .GetType()
                .GetField("isPlayerSpawned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(playerStart, false);

            // Act
            gameManager.Start();

            // Assert
            mockGameController.Verify(x => x.OnPossess(It.IsAny<PlayerView>()), Times.Once);
            mockGameController.Verify(x => x.Start(), Times.Once);
        }

        [Test]
        public void Dispose_ShouldSetIsStartedToFalse()
        {
            // Arrange
            gameManager.Start();

            // Act
            gameManager.Dispose();

            // Assert
            // Disposeを呼び出した後にStartを再実行できるかどうか確認することでisStartedがfalseになることを確認
            // PlayerStartの状態をリセットしてから再度Startを呼び出す
            typeof(PlayerStart).GetField("isPlayerSpawned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(playerStart, false);

            gameManager.Start();
            mockGameController.Verify(x => x.OnPossess(It.IsAny<PlayerView>()), Times.Exactly(2));
        }

        [Test]
        public void Register_ShouldRegisterPlayerStartInContainer()
        {
            // Arrange
            var builder = new ContainerBuilder();
    
            // テスト用のGameObjectを作成し、PlayerStartコンポーネントを追加
            var playerStartGameObject = new GameObject("PlayerStartTestObject");
            var start = playerStartGameObject.AddComponent<PlayerStart>();
    
            // PlayerStart コンポーネントを直接登録する
            builder.RegisterInstance(start).As<PlayerStart>();
    
            // Act
            var resolver = builder.Build();

            // Assert
            Assert.DoesNotThrow(() => resolver.Resolve<PlayerStart>());
        }
    }
}