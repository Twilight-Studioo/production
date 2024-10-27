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
        private static readonly int RimThreshold = Shader.PropertyToID("_RimThreashould");
        [SerializeField] private VisualEffect effect;
        [SerializeField] private bool isSwap;
        [SerializeField] private float highlightRimThreshold;

        // ReSharper disable once MemberCanBePrivate.Local
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private Material material;
        private Renderer targetRenderer;

        private void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            material = targetRenderer.material;
        }

        private void Update()
        {
            position.Value = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTrigger?.Invoke(other);
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => position;

        public Vector2 GetPosition() => position.Value;

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

        public void OnInSelectRange()
        {
            material.SetFloat(RimThreshold, highlightRimThreshold);
        }

        public void OnOutSelectRange()
        {
            material.SetFloat(RimThreshold, 1);
        }

        public event Action OnDestroyEvent;

        public void Dispose()
        {
            OnTrigger = null;
        }

        private event Action<Collider2D> OnTrigger;
        public bool IsSwap() => isSwap;
    }
}