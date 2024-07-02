#region

using System;
using System.Collections.Generic;
using System.Linq;
using Feature.Common;
using Feature.Model;
using Feature.View;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

#endregion

namespace Feature.Presenter
{
    public class SwapPresenter
    {
        private readonly CharacterParams characterParams;

        private readonly CompositeDisposable rememberItemPosition;
        private readonly SwapModel swapItemsModel;
        private Dictionary<Guid, SwapView> swapItemViews;

        [Inject]
        public SwapPresenter(
            SwapModel swapItemsModel,
            CharacterParams characterParams
        )
        {
            this.swapItemsModel = swapItemsModel;
            this.characterParams = characterParams;
            var list = Object.FindObjectsOfType<SwapView>().ToList();
            rememberItemPosition = new();
            AddItems(list);
        }

        public void Dispose()
        {
            rememberItemPosition.Clear();
        }

        private void AddItems(List<SwapView> items)
        {
            if (swapItemViews == null)
            {
                swapItemViews = new();
            }

            var dats = items.Select(item =>
            {
                var id = Guid.NewGuid();
                item.SetHighlight(false);
                item.Position
                    .Subscribe(_ => { swapItemsModel.UpdateItemPosition(id, item.Position.Value); })
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
                        Position = data.item.Position.Value,
                    }
                ).ToList());
        }

        public void RemoveItems(List<SwapView> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var swapItemViewBase in items)
            {
                var found = swapItemViews.First(x => x.Value == swapItemViewBase);
                swapItemsModel.RemoveItem(found.Key);
                swapItemViews.Remove(found.Key);
            }
        }

        public void Clear()
        {
            rememberItemPosition.Clear();
        }

        public void ResetSelector()
        {
            swapItemsModel.ResetSelector();
        }

        public void MoveSelector(Vector2 direction, Vector3 basePosition)
        {
            var item = swapItemsModel.GetCurrentItem();
            Debug.Log("item = " + item);
            var select = swapItemsModel.GetNearestItem(basePosition, direction, characterParams.canSwapDistance);
            Debug.Log("select = " + select);
            if (!select.HasValue)
            {
                return;
            }

            if (item.HasValue)
            {
                swapItemViews[item.Value.Id].SetHighlight(false);
            }

            swapItemViews[select.Value.Id].SetHighlight(true);
            swapItemsModel.SetItem(select.Value.Id);
        }

        public SwapView SelectItem()
        {
            var item = swapItemsModel.GetCurrentItem();
            if (!item.HasValue || item.Value.Id == Guid.Empty)
            {
                return null;
            }

            return swapItemViews[item.Value.Id];
        }
    }
}