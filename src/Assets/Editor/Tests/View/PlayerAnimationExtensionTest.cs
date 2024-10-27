#region

using Feature.Interface;
using Feature.View;
using Moq;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

#endregion

namespace Editor.Tests.View
{
    public class PlayerAnimationExtensionTest
    {
        private Mock<IAnimator> animatorMock;

        [SetUp]
        public void SetUp()
        {
            animatorMock = new();
            Assert.IsNotNull(animatorMock);
        }

        [Test]
        public void SetIsFallingTest()
        {
            animatorMock.Object.SetIsFalling(true);
            animatorMock.Verify(x => x.SetBool(PlayerAnimationExtension.IsFallingHash, true), Times.Once);

            animatorMock.Object.SetIsFalling(false);
            animatorMock.Verify(x => x.SetBool(PlayerAnimationExtension.IsFallingHash, false), Times.Once);
        }

        [Test]
        public void SetSpeedTest()
        {
            animatorMock.Object.SetSpeed(1.0f);
            animatorMock.Verify(x => x.SetFloat(PlayerAnimationExtension.SpeedHash, 1.0f), Times.Once);

            animatorMock.Object.SetSpeed(0.0f);
            animatorMock.Verify(x => x.SetFloat(PlayerAnimationExtension.SpeedHash, 0.0f), Times.Once);
        }

        [Test]
        public void SetJumpTest()
        {
            animatorMock.Object.OnJump();
            animatorMock.Verify(x => x.SetTrigger(PlayerAnimationExtension.OnJumpHash), Times.Once);
        }

        [Test]
        public void OnTakeDamageTest()
        {
            animatorMock.Object.OnTakeDamage();
            animatorMock.Verify(x => x.SetTrigger(PlayerAnimationExtension.OnTakeDamageHash), Times.Once);
        }

        [Test]
        public void OnAttackTest()
        {
            animatorMock.Object.OnAttack(0.5f);
            animatorMock.Verify(x => x.SetTrigger(PlayerAnimationExtension.OnAttackHash), Times.Once);
            animatorMock.Verify(x => x.SetBool(PlayerAnimationExtension.IsAttackingHash, true), Times.Once);
            animatorMock.Verify(x => x.SetBoolDelay(PlayerAnimationExtension.IsAttackingHash, false, 0.5f), Times.Once);
        }

        [Test]
        public void OnDaggerTest()
        {
            animatorMock.Object.OnDagger();
            animatorMock.Verify(x => x.SetTrigger(PlayerAnimationExtension.OnDaggerHash), Times.Once);
        }

        [Test]
        public void SetAttackComboCountTest()
        {
            animatorMock.Object.SetAttackComboCount(1);
            animatorMock.Verify(x => x.SetFloat(PlayerAnimationExtension.AttackComboCountHash, 1), Times.Once);

            animatorMock.Object.SetAttackComboCount(0);
            animatorMock.Verify(x => x.SetFloat(PlayerAnimationExtension.AttackComboCountHash, 0), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            animatorMock = null;
        }
    }
}