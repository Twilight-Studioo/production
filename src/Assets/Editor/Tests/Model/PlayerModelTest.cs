using System;
using Editor.Tests.Common;
using Feature.Common.Parameter;
using Feature.Model;
using Moq;
using NUnit.Framework;
using UnityEngine;
using VContainer;
using Assert = UnityEngine.Assertions.Assert;
namespace Editor.Tests.Model
{
    public class PlayerModelTest: SingleInjectionClassTest<PlayerModel>
    {
        private CharacterParams characterParams;
        protected override void RegisterDependencies(IContainerBuilder builder)
        {
            characterParams = EditorTestEx.CreateScriptableObject<CharacterParams>();
            characterParams.maxHasStamina = 30;
            characterParams.enterSwapUseStamina = 10;
            characterParams.swapExecUseStamina = 10;
            builder.RegisterInstance(characterParams);
        }
        
        [
            TestCase("CanEndSwap"), 
            TestCase("CanStartSwap"),
            TestCase("CanDagger"),
            TestCase("SwapStamina"),
            TestCase("DaggerStamina"),
            TestCase("Health"),
            TestCase("State"),
            TestCase("Position"),
        ]
        public void Propertyの個々のフィールドが初期化されているか(string propertyName)
        {
            Assert.IsNotNull(TestInstance);

            var property = TestInstance.GetType().GetField(propertyName);
            Assert.IsNotNull(property.GetValue(TestInstance), $"{propertyName} should not be null");
        }

        [Test]
        public void CanStartSwapが更新されているか()
        {
            Assert.IsFalse(TestInstance.CanStartSwap.Value);
        }
        
        [Test]
        public void ChangeStateが反映されているか()
        {
            TestInstance.Start();
            Assert.AreEqual(TestInstance.State.Value, PlayerModel.PlayerState.Idle, "State should be Idle at the start.");
            TestInstance.ChangeState(PlayerModel.PlayerState.DoSwap);
            Assert.AreEqual(TestInstance.State.Value, PlayerModel.PlayerState.DoSwap, "State should be DoSwap after ChangeState.");
        }
        
        [Test]
        public void 削除後にDisposableがDisposeされているか()
        {
            var mockSwapUseStaminaSubscription = new Mock<IDisposable>();
            var mockRecoverStaminaSubscription = new Mock<IDisposable>();
            var mockUseDaggerUseStamina = new Mock<IDisposable>();
            typeof(PlayerModel).GetField("swapUseStaminaSubscription", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(TestInstance, mockSwapUseStaminaSubscription.Object);
            typeof(PlayerModel).GetField("recoverStaminaSubscription", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(TestInstance, mockRecoverStaminaSubscription.Object);
            typeof(PlayerModel).GetField("useDaggerUseStamina", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(TestInstance, mockUseDaggerUseStamina.Object);
            
            TestInstance.Dispose();
            
            // Assert
            mockSwapUseStaminaSubscription.Verify(m => m.Dispose(), Times.Once, "swapUseStaminaSubscription should be disposed once.");
            mockRecoverStaminaSubscription.Verify(m => m.Dispose(), Times.Once, "recoverStaminaSubscription should be disposed once.");
            mockUseDaggerUseStamina.Verify(m => m.Dispose(), Times.Once, "useDaggerUseStamina should be disposed once.");
            
        }
        
        [Test]
        public void TakeDamageでダメージを与えられるか()
        {
            TestInstance.Start();
            var health = TestInstance.Health.Value;
            TestInstance.TakeDamage(10);
            Assert.AreEqual(TestInstance.Health.Value, health - 10, "Health should be 90 after taking 10 damage.");
            TestInstance.TakeDamage(10000);
            Assert.AreEqual(TestInstance.Health.Value, 0, "想定以上のダメージでもHealthは0以下にはならない");
        }
        
        [Test]
        public void UpdatePositionで座標が更新されているか()
        {
            TestInstance.Start();
            TestInstance.UpdatePosition(Vector3.one);
            Assert.AreEqual(TestInstance.Position.Value, Vector3.one, "Position should be Vector3.one after UpdatePosition.");
        }
        
        [Test]
        public void UpdatePositionでForwardが更新されているか()
        {
            TestInstance.Start();
            TestInstance.UpdatePosition(new(12, 0, 0));
            var pos = TestInstance.Position.Value;
            TestInstance.UpdatePosition(Vector3.one);
            var requiredForward = (Vector3.one - pos).normalized;
            Assert.AreEqual(TestInstance.Forward, requiredForward, "Forward should be normalized direction from old position to new position.");
        }
    }

}