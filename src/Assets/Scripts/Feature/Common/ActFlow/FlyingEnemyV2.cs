#region

using System;
using Core.Utilities;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("FlyingEnemyV2")]
    public class FlyingEnemyV2Action : FixedUpdatedAction, IGizmoDrawable, IDisposable
    {
        private const float HoverAmplitude = 0.6f; // 揺れの振幅
        private const float HoverFrequency = 2f; // 揺れの周波数
        private const float MinHeightFromGround = 1f; // 地面からの最低高さ
        private float hoverTime;
        private LayerMask obstacleMask;

        private Vector3 previousMoveForce;

        // 内部変数
        private Rigidbody rb;

        // パラメータ (ActionParameterで外部設定可能)
        [ActionParameter("Power")] private float Power { get; set; }
        [ActionParameter("Distance")] private float Distance { get; set; } = 5f;

        [ActionParameter("MaxHeightFromGround")]
        private float MaxHeightFromGround { get; set; } = 20f;

        [ActionParameter("MinDistanceToCeiling")]
        private float MinDistanceToCeiling { get; set; } = 5f;

        [ActionParameter("Target")] private Transform Player { get; set; }

        public void Dispose()
        {
            GizmoManager.Instance.Unregister(this);
        }

        public void DrawGizmos()
        {
            if (Owner == null)
            {
                GizmoManager.Instance.Unregister(this);
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Owner.transform.position, Distance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                Owner.transform.position,
                Owner.transform.position + Vector3.down * MaxHeightFromGround);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                Owner.transform.position,
                Owner.transform.position + Vector3.up * MinDistanceToCeiling);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                Owner.transform.position,
                Owner.transform.position + previousMoveForce * 2f);
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Distance = 5f;
            MaxHeightFromGround = 20f;
            MinDistanceToCeiling = 5f;
            Power = 5f;
            Player = null;
            GizmoManager.Instance.Register(this);
        }

        protected override void Start()
        {
            rb = Owner.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbodyがアタッチされていません: " + Owner.name);
            }

            if (Player == null)
            {
                Player = ObjectFactory.Instance.FindPlayer()?.transform;
            }

            obstacleMask = LayerMask.GetMask("Default", "Ground");
        }

        protected override void FixedUpdate()
        {
            if (rb == null)
            {
                return;
            }

            var hoverForce = Hover();
            var maintainDistanceForce = MaintainDistance();
            var adjustHeightForce = AdjustHeight();

            var totalForce = hoverForce + maintainDistanceForce + adjustHeightForce;
            previousMoveForce = totalForce;
            rb.velocity = totalForce;
        }

        protected override bool CheckIfEnd() => false;

        // その場に揺れる動作
        private Vector3 Hover()
        {
            hoverTime += Time.fixedDeltaTime;
            var offset = Mathf.Sin(hoverTime * HoverFrequency) * HoverAmplitude;
            return Vector3.up * offset;
        }

        // プレイヤーとの距離を保つ動作
        private Vector3 MaintainDistance()
        {
            if (Player == null)
            {
                return Vector3.zero;
            }

            var ownerPosition = Owner.transform.position;
            var playerPosition = Player.position;
            var directionToPlayer = (playerPosition - ownerPosition).normalized;
            var distanceToPlayer = Vector3.Distance(ownerPosition, playerPosition);

            // 距離の差を計算
            var distanceDifference = distanceToPlayer - Distance;

            // 移動力を距離の差に比例させる（負の値も考慮）
            var moveForce = directionToPlayer *
                            (Power * Random.Range(0.8f, 1.0f)) * (distanceDifference / Distance);

            return moveForce;
        }

        // 高さを調整する動作
        private Vector3 AdjustHeight()
        {
            // 地面までの距離を取得
            var distanceToGround = Mathf.Infinity;
            if (Physics.Raycast(Owner.transform.position, Vector3.down, out var groundHit, Mathf.Infinity,
                    obstacleMask))
            {
                distanceToGround = groundHit.distance;
            }

            // 天井までの距離を取得
            var distanceToCeiling = Mathf.Infinity;
            if (Physics.Raycast(Owner.transform.position, Vector3.up, out var ceilingHit, Mathf.Infinity, obstacleMask))
            {
                distanceToCeiling = ceilingHit.distance;
            }

            // 現在の高さを地面からの距離で計算
            var currentHeightAboveGround = distanceToGround;

            // 目標とする高さを計算
            var desiredHeightAboveGround = Mathf.Clamp(
                MaxHeightFromGround / 2,
                MinHeightFromGround,
                MaxHeightFromGround);

            // 天井との距離を考慮して目標高さを調整
            var maxAllowedHeight = distanceToGround + distanceToCeiling - MinDistanceToCeiling;
            desiredHeightAboveGround = Mathf.Min(desiredHeightAboveGround, maxAllowedHeight - distanceToGround);

            // 目標高さが地面からの最低高さよりも小さくならないようにする
            desiredHeightAboveGround = Mathf.Max(desiredHeightAboveGround, MinHeightFromGround);

            // 高さの差を計算
            var heightDifference = desiredHeightAboveGround - currentHeightAboveGround;

            // 高さの調整力を計算
            var adjustment = Vector3.up * Power * (heightDifference / MaxHeightFromGround);

            return adjustment;
        }
    }
}