#region

using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

#endregion

namespace Editor.Tests.Common
{
    public abstract class GameObjectComponentTest<T> where T : Component
    {
        protected GameObject GameObject;
        protected T TestInstance;

        [SetUp]
        public void SetUp()
        {
            // ここにテストのセットアップ処理を記述
            GameObject = new();
            TestInstance = GameObject.AddComponent<T>();
        }

        [Test]
        public void インスタンス作成が成功しているか()
        {
            Assert.IsNotNull(GameObject);
            Assert.IsNotNull(TestInstance);
        }
    }
}