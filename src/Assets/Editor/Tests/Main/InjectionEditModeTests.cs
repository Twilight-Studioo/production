#region

using System;
using System.Reflection;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.View;
using Main.Controller;
using Moq;
using NUnit.Framework;
using UnityEngine;
using VContainer;

#endregion

namespace Editor.Tests.Main
{
    public class InjectionEditModeTests
    {
        private GameManager gameManager;
        private Mock<IGameController> mockGameController;
        private Mock<IInputController> mockInputController;
        private GameObject playerGameObject;
        private GameObject playerPrefab;
        private PlayerStart playerStart;
        private PlayerView playerView;

        [SetUp]
        public void SetUp()
        {
            // 依存関係のモックを作成
            mockGameController = new();
            mockInputController = new();

            // テスト用のGameObjectとPlayerStartコンポーネントを作成
            var playerStartGameObject = new GameObject();
            playerStart = playerStartGameObject.AddComponent<PlayerStart>();

            // テスト用のプレイヤーのPrefabとそのPlayerViewコンポーネントを作成
            playerGameObject = new("Player");
            playerView = playerGameObject.AddComponent<PlayerView>();
            playerPrefab = playerGameObject;

            // PlayerStartにプレイヤープレハブを設定
            playerStart
                .GetType()
                .GetField("playerRef", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(playerStart, playerPrefab);

            // GameManagerのインスタンスを作成
            gameManager = new(playerStart, mockGameController.Object, mockInputController.Object);
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
                .GetField("isPlayerSpawned", BindingFlags.NonPublic | BindingFlags.Instance)
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
                .GetField("isPlayerSpawned", BindingFlags.NonPublic | BindingFlags.Instance)
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
            typeof(PlayerStart).GetField("isPlayerSpawned", BindingFlags.NonPublic | BindingFlags.Instance)
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