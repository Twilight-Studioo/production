#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Core.Utilities.Health
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject healthBarPrefab;
        [SerializeField]
        private GameObject canvas;
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
            ObjectFactory.OnObjectCreated += HandleNewObject;
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
            if (collider != null)
            {
                AttachHealthBarToCollider(healthObject, collider);
            }
            else
            {
                AttachHealthBarToTransform(healthObject, @object.transform);
            }
        }

        private void AttachHealthBarToCollider(IHealthBar healthObject, Collider collider)
        {
            var healthBar = Instantiate(healthBarPrefab, collider.transform);
            healthBar.transform.SetParent(canvas.transform);
            healthBar.GetComponent<HealthBar>().Initialize(healthObject);
            var follow = healthBar.GetComponent<FollowObject>();
            follow.canvas = canvas.GetComponent<Canvas>();
            follow.target = collider.transform;
            follow.offset = Vector3.up * collider.bounds.size.y;
        }

        private void AttachHealthBarToTransform(IHealthBar healthObject, Transform transform)
        {
            var healthBar = Instantiate(healthBarPrefab, transform);
            healthBar.transform.SetParent(canvas.transform);
            healthBar.GetComponent<HealthBar>().Initialize(healthObject);
            var follow = healthBar.GetComponent<FollowObject>();
            follow.canvas = canvas.GetComponent<Canvas>();
            follow.target = transform;
            follow.offset = Vector3.up * 2f;
        }
    }
}