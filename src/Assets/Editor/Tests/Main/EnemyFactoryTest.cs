#region

using System;
using System.Collections.Generic;
using Editor.Tests.Common;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Component.Environment;
using Main.Factory;
using NUnit.Framework;
using UnityEngine;

#endregion

namespace Editor.Tests.Main
{
    public class EnemyFactoryTest
    {
        private EnemiesSetting enemiesSetting;
        private EnemyFactory enemyFactory;
        private List<EnemyStart> enemyStarts;

        [SetUp]
        public void SetUp()
        {
            // EnemiesSettingのインスタンスをScriptableObjectとして作成
            enemiesSetting = ScriptableObject.CreateInstance<EnemiesSetting>();
            enemiesSetting.reference = new();

            // EnemyStartのリストを準備
            enemyStarts = new()
            {
                new GameObject().AddComponent<EnemyStart>(),
            };

            // EnemyFactoryオブジェクトの生成
            var gameObject = new GameObject();
            enemyFactory = gameObject.AddComponent<EnemyFactory>();

            // ReflectionHelperを使ってprivateフィールドにEnemiesSettingを設定
            ReflectionHelper.SetPrivateField(enemyFactory, "settings", enemiesSetting);
        }

        [Test]
        public void Subscribe_SettingsIsNull_ThrowsException()
        {
            // Arrange
            ReflectionHelper.SetPrivateField<EnemiesSetting>(enemyFactory, "settings", null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => enemyFactory.Subscribe());
            Assert.AreEqual("Settings is not set", exception.Message);
        }

        [Test]
        public void Subscribe_ValidatesSettings()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => enemyFactory.Subscribe());
            Assert.AreEqual("EnemyType SimpleEnemy1 is not found in settings", exception.Message);
        }

        [Test]
        public void 設定に重複したpairをチェックする()
        {
            // Arrange
            var enemyStartMock = enemyStarts[0];
            ReflectionHelper.SetPrivateField(enemyStartMock, "spawnEnemyType", EnemyType.SimpleEnemy1);

            // EnemiesSettingの設定を調整
            enemiesSetting.reference.Add(new() { type = EnemyType.SimpleEnemy1, reference = new(), });
            enemiesSetting.reference.Add(new() { type = EnemyType.SimpleEnemy1, reference = new(), });

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => enemyFactory.Subscribe());
            Assert.AreEqual($"Duplicate EnemyType {EnemyType.SimpleEnemy1.ToString()}", exception.Message);
        }

        [Test]
        public void Subscribe_EnemyがIEnemyを継承しているかをチェックする()
        {
            // Arrange
            var enemyStartMock = enemyStarts[0];
            ReflectionHelper.SetPrivateField(enemyStartMock, "spawnEnemyType", EnemyType.SimpleEnemy1);

            // EnemiesSettingの設定を調整
            enemiesSetting.reference.Add(new() { type = EnemyType.None, reference = new(), });

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => enemyFactory.Subscribe());
            Assert.AreEqual("EnemyType None is not implement IEnemy", exception.Message);
        }
    }
}