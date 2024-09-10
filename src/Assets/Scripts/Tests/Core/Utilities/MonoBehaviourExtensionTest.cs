#region

using System;
using System.Collections;
using Core.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

#endregion

namespace Tests.Core.Utilities
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        // コルーチンを呼び出すメソッドを作成
        public Coroutine RunDelayMethod(float waitTime, Action action) =>
            StartCoroutine(this.DelayMethod(waitTime, action));

        public Coroutine RunDelayMethod<T>(float waitTime, Action<T> action, T param) =>
            StartCoroutine(this.DelayMethod(waitTime, action, param));

        public Coroutine RunDelayMethod<T1, T2>(float waitTime, Action<T1, T2> action, T1 param1, T2 param2) =>
            StartCoroutine(this.DelayMethod(waitTime, action, param1, param2));
    }

    public class MonoBehaviourExtensionTest
    {
        private TestMonoBehaviour testMonoBehaviour;

        [SetUp]
        public void SetUp()
        {
            // テスト用のゲームオブジェクトを作成し、テスト用の MonoBehaviour を追加
            var testGameObject = new GameObject();
            testMonoBehaviour = testGameObject.AddComponent<TestMonoBehaviour>();
        }

        [UnityTest]
        public IEnumerator DelayMethod_SimpleAction_ShouldBeCalledAfterDelay()
        {
            var actionCalled = false;

            // コルーチンを実行
            yield return testMonoBehaviour.RunDelayMethod(0.5f, () => actionCalled = true);

            // アクションが呼び出されたか確認
            Assert.IsTrue(actionCalled);
        }

        [UnityTest]
        public IEnumerator DelayMethod_WithParameters_ShouldBeCalledWithCorrectParameters()
        {
            var actionCalled = false;
            var receivedInt = 0;
            var receivedString = "";

            // アクションの作成
            Action<int, string> action = (intValue, stringValue) =>
            {
                actionCalled = true;
                receivedInt = intValue;
                receivedString = stringValue;
            };

            // コルーチンを実行
            yield return testMonoBehaviour.RunDelayMethod(0.5f, action, 42, "Hello");

            // アクションが呼び出されたか確認
            Assert.IsTrue(actionCalled);
            Assert.AreEqual(42, receivedInt);
            Assert.AreEqual("Hello", receivedString);
        }

        [TearDown]
        public void TearDown()
        {
            // テスト用のゲームオブジェクトを破棄
            Object.Destroy(testMonoBehaviour.gameObject);
        }
    }
}