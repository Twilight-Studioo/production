#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Core.Utilities.Health
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarPrefab;

        [SerializeField] private GameObject canvas;

        private readonly List<IHealthBar> trackedHealthBars = new();

        private void Awake()
        {
            SubscribeToNewObjectEvents();
        }

        private void Start()
        {
            var healthObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IHealthBar>().ToArray();
            foreach (var healthObject in healthObjects)
            {
                AttachHealthBar(healthObject);
                trackedHealthBars.Add(healthObject);
            }
        }

        private void SubscribeToNewObjectEvents()
        {
            // イベントサブスクライブの例
            ObjectFactory.Instance.OnObjectCreated += HandleNewObject;
        }

        private void HandleNewObject(GameObject newObj)
        {
            var healthComponent = newObj.GetComponent<IHealthBar>();
            if (healthComponent != null)
            {
                AttachHealthBar(healthComponent);
                trackedHealthBars.Add(healthComponent);
            }
        }

        private void AttachHealthBar(IHealthBar healthObject)
        {
            var @object = healthObject as MonoBehaviour;
            if (@object == null)
            {
                return;
            }

            var collider = @object.GetComponent<Collider>();
            GameObject bar;
            if (collider != null)
            {
                bar = AttachHealthBarToCollider(collider);
            }
            else
            {
                bar = AttachHealthBarToTransform(@object.transform);
            }

            healthObject.OnRemoveEvent += () =>
            {
                trackedHealthBars.Remove(healthObject);
                Destroy(bar);
            };
            bar.transform.SetParent(canvas.transform);
            bar.GetComponent<HealthBar>().Initialize(healthObject);
        }

        private GameObject AttachHealthBarToCollider(Collider cl)
        {
            var healthBar = Instantiate(healthBarPrefab, cl.transform);
            var follow = healthBar.GetComponent<FollowObject>();
            follow.canvas = canvas.GetComponent<Canvas>();
            follow.target = cl.transform;
            follow.offset = Vector3.up * cl.bounds.size.y;
            return healthBar;
        }

        private GameObject AttachHealthBarToTransform(Transform tf)
        {
            var healthBar = Instantiate(healthBarPrefab, tf);
            var follow = healthBar.GetComponent<FollowObject>();
            follow.canvas = canvas.GetComponent<Canvas>();
            follow.target = tf;
            follow.offset = Vector3.up * 2f;

            return healthBar;
        }
    }
}