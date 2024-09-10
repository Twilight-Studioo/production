using System;
using UniRx;
using UnityEngine;

namespace Feature.Interface
{
    public interface ISwappable
    {
        void OnSelected();
        
        void OnDeselected();
        
        IReadOnlyReactiveProperty<Vector2> GetPositionRef();
        
        Vector2 GetPosition();
        
        void OnSwap(Vector2 p);
        
        public event Action OnDestroyEvent;
    }
}