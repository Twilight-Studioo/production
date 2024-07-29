#region

using System.Linq;
using UnityEngine;

#endregion

namespace Core.Utilities.Health
{
    public sealed class HealthBarManager
    {
        public GameObject healthBarPrefab;

        public void Start()
        {
            // シーン内の全てのオブジェクトをチェック
            var healthObjects = Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IHealthBar>().ToArray();
            foreach (var healthObject in healthObjects)
            {
                AttachHealthBar(healthObject);
            }
        }

        private void AttachHealthBar(IHealthBar healthObject)
        {
            var @object = healthObject as MonoBehaviour;
            if (@object == null)
            {
                return;
            }

            // collisionがある場合はcollisionの上にHPバーを表示する
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
            var healthBar = Object.Instantiate(healthBarPrefab, collider.transform);
            healthBar.transform.localPosition = Vector3.up * collider.bounds.size.y;
            healthBar.GetComponent<HealthBar>().Initialize(healthObject);
        }

        private void AttachHealthBarToTransform(IHealthBar healthObject, Transform transform)
        {
            var healthBar = Object.Instantiate(healthBarPrefab, transform);
            healthBar.transform.localPosition = Vector3.up * 2f;
            healthBar.GetComponent<HealthBar>().Initialize(healthObject);
        }
    }
}