using System;
using System.Collections.Generic;
using Editor.Tests.Common;
using Feature.Model;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Editor.Tests.Model
{
    public class SwapModelTest: SingleInjectionClassTest<SwapModel>
    {

        [Test]
        public void SwapItem_違う値ならハッシュコードが一致しない()
        {
            SwapItem a = new(Guid.NewGuid(), new(1, 2, 3), new());
            SwapItem b = new(Guid.NewGuid(), new(4, 5, 6), new());
            // ハッシュコードが一致するか
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void SwapItem_EqualsがIdのみで識別する()
        {
            SwapItem a = new(Guid.NewGuid(), new(1, 2, 3), new());
            SwapItem b = new(Guid.NewGuid(), new(4, 5, 6), new());
            SwapItem c = new(a.Id, new(7, 8, 9), new());
            // 違うID同士はequalsがfalse
            Assert.IsFalse(a.Equals(b));
            // 同じID同士はequalsがtrue
            Assert.IsTrue(a.Equals(c));
        }

        [Test]
        public void 初期値で選択中のアイテムは存在しない()
        {
            var items = new List<SwapItem>
            {
                new(),
                new(),
                new(),
            };
            Assert.IsTrue(TestInstance.GetCurrentItem() == null);
            TestInstance.AddItems(items);
            Assert.IsTrue(TestInstance.GetCurrentItem() == null);
        }
        
        [Test]
        public void アイテムを追加して選択中のアイテムを取得できる()
        {
            var items = new List<SwapItem>
            {
                new(Guid.NewGuid(), new(1, 2, 3), new()),
                new(Guid.NewGuid(), new(4, 5, 6), new()),
                new(Guid.NewGuid(), new(7, 8, 9), new()),
            };
            TestInstance.AddItems(items);
            TestInstance.SetItem(items[1].Id);
            var item = TestInstance.GetCurrentItem();
            Assert.IsTrue(item.HasValue);
            Assert.AreEqual(item.Value, items[1]);
        }
        
        [Test]
        public void ItemのPositionを更新できる()
        {
            var items = new List<SwapItem>
            {
                new(Guid.NewGuid(), new(1, 2, 3), new()),
                new(Guid.NewGuid(), new(4, 5, 6), new()),
                new(Guid.NewGuid(), new(7, 8, 9), new()),
            };
            TestInstance.AddItems(items);
            TestInstance.SetItem(items[1].Id);

            var currentItem = TestInstance.GetCurrentItem();
            Assert.IsTrue(currentItem.HasValue);
            // 初期値の確認
            Assert.AreEqual(currentItem.Value.Position, new(4, 5, 6));
    
            // 更新
            TestInstance.UpdateItemPosition(items[1].Id, new(10, 11, 12));
            currentItem = TestInstance.GetCurrentItem();
            Assert.IsTrue(currentItem.HasValue);
            // 更新後の確認
            Assert.AreEqual(currentItem.Value.Position, new(10, 11, 12));
        }
    }
}