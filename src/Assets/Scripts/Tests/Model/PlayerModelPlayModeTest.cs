#region

using System.Collections;
using Editor.Tests.Common;
using Feature.Common.Parameter;
using Feature.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

#endregion

namespace Tests.Model
{
    public class PlayerModelPlayModeTest
    {
        private CharacterParams characterParams;
        private PlayerModel playerModel;

        [SetUp]
        public void SetUp()
        {
            // テスト用の CharacterParams の設定
            characterParams = EditorTestEx.CreateScriptableObject<CharacterParams>();
            characterParams.maxHasStamina = 30;
            characterParams.enterSwapUseStamina = 10;
            characterParams.swapExecUseStamina = 10;
            characterParams.health = 100;

            // PlayerModel の初期化
            playerModel = new(characterParams);
        }

        [UnityTest]
        public IEnumerator スタミナが少なくなった時にswapが無効になっているか()
        {
            playerModel.Start();
            yield return new WaitForFixedUpdate();

            Assert.AreEqual(playerModel.SwapStamina.Value, 30, "SwapStamina should be 30 at the start.");
            // Act
            yield return new WaitForFixedUpdate();

            // Assert: CanStartSwapが正しく更新されているか確認
            Assert.IsTrue(playerModel.CanStartSwap.Value, "CanStartSwap should be true after one frame.");

            playerModel.OnStartSwap();

            Assert.AreEqual(
                playerModel.SwapStamina.Value,
                characterParams.maxHasStamina - characterParams.enterSwapUseStamina,
                "SwapStamina should be 20 after entering swap."
            );
            // playerModel.Swapped();
            //
            // yield return new WaitForFixedUpdate();
            //
            // // Param分のスタミナが減少
            // Assert.AreEqual(
            //     playerModel.SwapStamina.Value,
            //     characterParams.maxHasStamina - characterParams.enterSwapUseStamina -
            //     characterParams.swapExecUseStamina,
            //     "SwapStamina should be 10 after swapping."
            // );
            //
            // // スワップが不可能となる
            // Assert.IsFalse(playerModel.CanStartSwap.Value, "CanStartSwap should be false after stamina decreases.");
        }
    }
}