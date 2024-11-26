using System;
using Feature.Interface;
using UniRx;
using UnityEngine;

namespace Feature.Component.Enemy.SmasherEx
{
    /// <summary>
    /// 瓦礫オブジェクト
    /// <warning>Smasherでのみ使用</warning>
    /// </summary>
    public class DebrisObject: MonoBehaviour, ISwappable
    {
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private void Update()
        {
            position.Value = transform.position;
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => position;

        public Vector2 GetPosition() => position.Value;

        public void OnSwap(Vector2 p)
        {
            transform.position = p;
        }

        public event Action OnDestroyEvent;
        public void OnInSelectRange()
        {
            
        }

        public void OnOutSelectRange()
        {
            
        }

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}