﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using Feature.Common.Parameter;
using Feature.Component;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Model;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

#endregion

namespace Feature.Presenter
{
    public class SwapPresenter : IDisposable
    {
        private readonly CharacterParams characterParams;
        private readonly CompositeDisposable rememberItemPosition;
        private readonly SelectorEffect selectorEffect;
        private readonly SwapEffectFactory swapEffectFactory;
        private readonly SwapModel swapItemsModel;
        private Dictionary<Guid, ISwappable> swapItemViews;

        [Inject]
        public SwapPresenter(
            SwapModel swapItemsModel,
            CharacterParams characterParams,
            SwapEffectFactory swapEffectFactory,
            SelectorEffect selectorEffect
        )
        {
            this.swapItemsModel = swapItemsModel;
            this.characterParams = characterParams;
            this.swapEffectFactory = swapEffectFactory;
            this.selectorEffect = selectorEffect;
            var list = Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISwappable>().ToList();
            rememberItemPosition = new();
            AddItems(list);
        }

        public void Dispose()
        {
            rememberItemPosition.Clear();
        }

        public void RemoveItem(ISwappable item)
        {
            if (swapItemViews == null)
            {
                return;
            }

            var itemKey = swapItemViews.FirstOrDefault(x => x.Value == item).Key;
            if (itemKey == Guid.Empty)
            {
                return;
            }

            swapItemViews.Remove(itemKey);
            swapItemsModel.RemoveItem(itemKey);
        }

        public void AddItem(ISwappable item)
        {
            AddItems(new() { item, });
        }

        private void AddItems(List<ISwappable> items)
        {
            if (swapItemViews == null)
            {
                swapItemViews = new();
            }

            var dats = items.Select(item =>
            {
                var id = Guid.NewGuid();
                item.OnDeselected();
                item.OnDestroyEvent += () => { RemoveItem(item); };
                item.GetPositionRef()
                    .Subscribe(_ => { swapItemsModel.UpdateItemPosition(id, item.GetPosition()); })
                    .AddTo(rememberItemPosition);
                return (id, item);
            }).ToList();

            if (dats.Any(valueTuple => !swapItemViews.TryAdd(valueTuple.id, valueTuple.item)))
            {
                throw new InvalidOperationException("Failed to add items.");
            }

            swapItemsModel.AddItems(
                dats.Select(data => new SwapItem
                    {
                        Id = data.id,
                        Position = data.item.GetPosition(),
                    }
                ).ToList());
        }


        public void Clear()
        {
            rememberItemPosition.Clear();
        }

        public void ResetSelector()
        {
            var item = swapItemsModel.GetCurrentItem();
            if (item.HasValue)
            {
                SelectorStop();
                swapItemViews[item.Value.Id].OnDeselected();
            }

            swapItemsModel.ResetSelector();
        }

        public void MoveSelector(Vector2 direction, Vector3 basePosition)
        {
            var item = swapItemsModel.GetCurrentItem();
            var select = swapItemsModel.GetNearestItem(basePosition, direction, characterParams.canSwapDistance);
            if (!select.HasValue)
            {
                return;
            }

            if (item.HasValue)
            {
                swapItemViews[item.Value.Id].OnDeselected();
            }

            swapItemViews[select.Value.Id].OnSelected();
            selectorEffect.Selector(select.Value.Position);
            swapItemsModel.SetItem(select.Value.Id);
        }

        public ISwappable SelectItem()
        {
            var item = swapItemsModel.GetCurrentItem();
            if (!item.HasValue || item.Value.Id == Guid.Empty)
            {
                return null;
            }

            return swapItemViews[item.Value.Id];
        }

        public void Swap(Vector3 pos1, Vector3 pos2)
        {
            swapEffectFactory.PlayEffectAtPosition(pos1);
            swapEffectFactory.PlayEffectAtPosition(pos2);
        }

        public void InRangeHighlight(Vector3 basePosition, bool isSwap)
        {
            UpdateItemSelection(basePosition, isSwap);
        }

        private void UpdateItemSelection(Vector3 basePosition, bool isSelected)
        {
            var items = swapItemsModel.GetItemsInRange(basePosition, characterParams.canSwapDistance);
            foreach (var i in items)
            {
                var item = swapItemViews[i.Id];
                if (item == null)
                {
                    continue;
                }

                if (isSelected)
                {
                    item.OnInSelectRange();
                }
                else
                {
                    item.OnOutSelectRange();
                }
            }
        }

        public void SelectorStop()
        {
            selectorEffect.SelectorStop();
            var allItems = swapItemsModel.Items;
            foreach (var item in allItems)
            {
                var itemObj = swapItemViews[item.Id];
                if (itemObj == null)
                {
                    continue;
                }

                itemObj.OnOutSelectRange();
            }
        }
    }
}