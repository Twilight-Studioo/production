    #region

    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.VFX;

    #endregion

    namespace Feature.View
    {
        [RequireComponent(typeof(Material))]
        public class SwapView : MonoBehaviour
        {
            // ReSharper disable once MemberCanBePrivate.Local
            public readonly IReactiveProperty<Vector2> Position = new ReactiveProperty<Vector2>();

            [NonSerialized] protected bool IsActive;
            [SerializeField] private VisualEffect effect;
            [SerializeField] private float onStopTime = 1f;
            [SerializeField] private bool isSwap = false;
            
            private Material material;
            private Renderer targetRenderer;
            [SerializeField] private float hilightRimThreashold = 0f;

            protected virtual void Start()
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

            public virtual void Dispose()
            {
                OnDestroy = null;
                OnTrigger = null;
            }

            public event Action OnDestroy;

            protected event Action<Collider2D> OnTrigger;

            public void SetPosition(Vector2 position)
            {
                transform.position = position;
            }

            public void SetHighlight(bool isHighlight)
            {
                if (material == null)
                {
                    return;
                }
                if (isHighlight == true)
                {
                    //選択されている場合強調表示
                    material.SetFloat("_RimThreashould", hilightRimThreashold);
                }
                else
                {
                    material.SetFloat("_RimThreashould", 1);
                }
            }

            protected void Delete()
            {
                OnDestroy?.Invoke();
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
            public void PlayVFX()
            {
                if (effect != null)
                {
                    effect.SendEvent("OnPlay");
                    Invoke("StopVFX", onStopTime);
                }
            }

            public void StopVFX()
            {
                if (effect != null)
                {
                    effect.SendEvent("OnStop");
                }
            }

            public void StartSwap()
            {
                isSwap = true;
            }

            public void EndSwap()
            {
                isSwap = false;
                Debug.Log($"EndSwap called. isSwapActive = {isSwap}");
            }
        
            public bool IsSwap()
            {
                return isSwap;
            }
        }
    }