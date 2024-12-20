#region

using System;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Interface
{
    public interface ISwappable
    {
        public void OnSelected();

        public void OnDeselected();

        IReadOnlyReactiveProperty<Vector2> GetPositionRef();

        Vector2 GetPosition();

        void OnSwap(Vector2 p);

        public event Action OnDestroyEvent;

        void OnInSelectRange();

        void OnOutSelectRange();

        public void Delete();
    }
}