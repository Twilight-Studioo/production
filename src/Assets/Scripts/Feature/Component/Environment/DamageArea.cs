#region

using Core.Utilities;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Component.Environment
{
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private uint damege = 5;
        private BoxCollider boxCollider;


        private float lastUpdatedAt;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider is null)
            {
                Debug.LogWarning("BoxColliderがアタッチされていません。");
            }
        }

        private void FixedUpdate()
        {
            if (Time.time - lastUpdatedAt < 0.4f)
            {
                return;
            }

            lastUpdatedAt = Time.time;

            // オブジェクトのスケールとコライダーサイズからワールドサイズを計算
            var boxSize = Vector3.Scale(boxCollider.size, transform.lossyScale) / 2f;

            // BoxCastの方向は、forwardベクトルを使用
            var direction = transform.forward;

            // BoxCastを実行して衝突したすべてのオブジェクトを取得
            var hits = transform.GetBoxCastAll(boxSize, direction, 0f, 10);

            // 結果を処理
            foreach (var hit in hits)
            {
                if (hit.collider is null)
                {
                    continue;
                }

                if (hit.collider.CompareTag("SwapItem"))
                {
                    var obj = hit.collider.gameObject.GetComponent<ISwappable>();
                    obj?.Delete();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")
                || other.gameObject.CompareTag("Enemy"))
            {
                var obj = other.gameObject.GetComponent<IDamaged>();

                obj?.OnDamage(damege, transform.position, transform);
            }
        }
    }
}