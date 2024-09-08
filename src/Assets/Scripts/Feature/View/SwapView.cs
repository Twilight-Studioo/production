#region

using System;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    [RequireComponent(typeof(Material))]
    public sealed class SwapView : MonoBehaviour, ISwappable
    {
        [SerializeField] private float hilightRimThreashold;

        // ReSharper disable once MemberCanBePrivate.Local
        public readonly IReactiveProperty<Vector2> Position = new ReactiveProperty<Vector2>();

        [NonSerialized] private bool IsActive;

        private Material material;
        private Renderer targetRenderer;

        private void Start()
        {
            IsActive = true;
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

        public void OnSelected()
        {
            SetHighlight(true);
        }
        
        public void OnDeselected()
        {
            SetHighlight(false);
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => Position;
        
        public Vector2 GetPosition() => Position.Value;

        public void Dispose()
        {
            OnTrigger = null;
        }

        private event Action<Collider2D> OnTrigger;

        private void SetHighlight(bool isHighlight)
        {
            if (material == null)
            {
                return;
            }

            if (isHighlight)
            {
                //選択されている場合強調表示
                material.SetFloat("_RimThreashould", hilightRimThreashold);
            }
            else
            {
                material.SetFloat("_RimThreashould", 1);
            }
        }

        private void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }

        //public void UpdateRimThreshold(Guid id, float newThreshold)
        /*{
            var item = swapItems.FirstOrDefault(x => x.Id == id);
            if (item.Renderer != null)
            {
                var material = item.Renderer.material;
                if (material.HasProperty("_RimThreshold"))
                {
                    material.SetFloat("_RimThreshold", newThreshold);
                }
            }
        }*/
        
        public void OnSwap(Vector2 position)
        {
            transform.position = position;
        }

        public event Action OnDestroyEvent;
    }
}