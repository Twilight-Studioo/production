#region

using System;
using Feature.Interface;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.View
{
    [RequireComponent(typeof(Material))]
    public sealed class SwapView : MonoBehaviour, ISwappable
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private bool isSwap;
        [SerializeField] private float hilightRimThreashold;

        // ReSharper disable once MemberCanBePrivate.Local
        public readonly IReactiveProperty<Vector2> Position = new ReactiveProperty<Vector2>();

        private Material material;
        private Renderer targetRenderer;

        private void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            material = targetRenderer.material;
        }

        private void Update()
        {
            Position.Value = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTrigger?.Invoke(other);
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => Position;
        
        public Vector2 GetPosition() => Position.Value;

        public void Dispose()
        {
            OnTrigger = null;
        }

        private event Action<Collider2D> OnTrigger;

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
        
        public void OnSelected()
        {
            
        }

        public void OnDeselected()
        {
        }
        
        public void OnSwap(Vector2 position)
        {
            transform.position = position;
        }
        public bool IsSwap() => isSwap;
        public void OnInSelectRange()
        {
            material.SetFloat("_RimThreashould", hilightRimThreashold);
        }
        
        public void OnOutSelectRange()
        {
            material.SetFloat("_RimThreashould", 1);
        }

        public event Action OnDestroyEvent;
    }
}