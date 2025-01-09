#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Feature.Model
{
    public struct SwapItem : IEquatable<SwapItem>
    {
        public Guid Id;
        public Vector3 Position;
        private readonly Renderer renderer;

        public SwapItem(Guid id, Vector3 position, Renderer renderer)
        {
            Id = id;
            Position = position;
            this.renderer = renderer;
        }

        public bool Equals(SwapItem other) => Id.Equals(other.Id);

        public override bool Equals(object obj) => obj is SwapItem other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, Position, renderer);
    }

    public class SwapModel
    {
        private Guid currentId = Guid.Empty;
        public List<SwapItem> Items { get; } = new();
        
        private readonly float canSelectDirectionRange = 45f;

        public void AddItems(List<SwapItem> items)
        {
            Items.AddRange(items);
        }

        public void RemoveItems(Predicate<Guid> match)
        {
            Items.RemoveAll(item =>
            {
                var found = match(item.Id);
                if (found && item.Id == currentId)
                {
                    currentId = Guid.Empty;
                }

                return found;
            });
        }

        public void RemoveItem(Guid id)
        {
            Items.RemoveAll(item => item.Id == id);
        }

        public void ResetSelector()
        {
            currentId = Guid.Empty;
        }

        public void UpdateItemPosition(Guid id, Vector3 position)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            var index = Items.FindIndex(x => x.Id == id);
            if (index < 0 || index >= Items.Count)
            {
                return;
            }

            var swapItem = Items[index];
            swapItem.Position = position;
            Items[index] = swapItem;
        }

        public void SetItem(Guid id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            currentId = id;
        }
#nullable enable
        public SwapItem? GetCurrentItem()
        {
            if (currentId == Guid.Empty)
            {
                return null;
            }

            try
            {
                return Items.First(x => x.Id == currentId);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns the closest object from a specified location in a preferred direction, but within a maximum distance
        /// </summary>
        /// <param name="position">The specified location from which we need to find the nearest object</param>
        /// <param name="direction">The preferred direction in which we need to find the object</param>
        /// <param name="maxDistance">The maximum distance from 'position' at which an item may be considered (squared distance) </param>
        /// <returns>The nearest object in the preferred direction within the maximum distance, null if no such item exists</returns>
        public SwapItem? GetNearestItem(Vector3 position, Vector3 direction, float maxDistance)
        {
            var nearestItem = (SwapItem?)null;
            var nearestDirDot = -1f;
            var nearestDistance = Mathf.Infinity;

            if (direction is { x: 0f, y: 0f })
            {
                return null;
            }

            direction.Normalize();

            var itemsInRange = Items
                .Where(item => Vector3.Distance(item.Position, position) < maxDistance)
                .ToList();

            foreach (var item in itemsInRange)
            {
                var itemDirection = (item.Position - position).normalized;
                var distance = Vector3.Distance(item.Position, position);
                var angleDot = Vector3.Dot(direction, itemDirection);

                var angle = Mathf.Acos(angleDot) * Mathf.Rad2Deg;

                if (angle <= canSelectDirectionRange)
                {
                    if (angleDot > nearestDirDot || (Mathf.Approximately(angleDot, nearestDirDot) && distance < nearestDistance))
                    {
                        nearestDirDot = angleDot;
                        nearestItem = item;
                        nearestDistance = distance;
                    }
                }
            }

            return nearestItem;
        }

        public List<SwapItem> GetItemsInRange(Vector3 position, float maxDistance)
        {
            var itemsInRange = Items
                .Where(item => Vector3.Distance(item.Position, position) < maxDistance)
                .ToList();
            return itemsInRange;
        }
    }
}