#region

using System;
using System.Collections.Generic;
using Core.Utilities;
using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using Feature.Common.Constants;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("FlyingEnemy")]
    public class FlyingEnemyAction : FixedUpdatedAction, IGizmoDrawable, IDisposable
    {
        // 移動速度と回避力
        private const float MoveSpeed = 10f; // seek時のspeed
        private const float AvoidanceForce = 15f; // 障害物回避時のforce
        private const float MaxForce = 20f; // 最大力
        private const float MaxSpeed = 10f; // 最大速度

        // 障害物検出範囲
        private const float DetectionRange = 10f; // 障害物検出範囲

        // フォーミングの設定
        private const float NeighborDistance = 5f;
        private const float SeparationDistance = 2f;

        // プレイヤーから離れる際の力
        private const float FleeForce = 10f;

        // 目標高度
        private const float TargetHeight = 14f; // 地面からドローンが到達すべき高さ

        // 上昇時の力
        private const float AscentForce = 7f;

        // レイキャストの高さ検出範囲
        private const float AscentDetectionRange = 2f;

        private const float MaxHeight = 25f; // 地面からドローンが到達できる最大高さ

        private const float AscentAvoidanceAngle = 45f; // 回避角度（左右どちらに回避するかを決定）

        private const float AscentAvoidanceForce = 10f; // 上昇時の回避力

        // 高度維持のための調整
        private bool limitAscentAboveMaxHeight = true; // 最大高度以上では上昇しないようにする
        private LayerMask obstacleMask;

        // Rigidbodyコンポーネント
        private Rigidbody rb;

        // プレイヤーのTransform
        [ActionParameter("Transform")] private Transform Player { get; set; }

        // プレイヤーとの最小距離
        [ActionParameter("PlayerMinDistance")] private float MinDistance { get; set; }

        public void Dispose()
        {
            GizmoManager.Instance.Unregister(this);
        }

        // デバッグ用にレイを可視化
        public void DrawGizmos()
        {
            // 障害物検出範囲
            Gizmos.color = Color.red;
            if (rb != null)
            {
                Gizmos.DrawLine(Owner.transform.position,
                    Owner.transform.position + rb.velocity.normalized * DetectionRange);
            }

            if (Owner == null)
            {
                GizmoManager.Instance.Unregister(this);
                return;
            }

            // フォーミングの範囲
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Owner.transform.position, NeighborDistance);
            Gizmos.DrawWireSphere(Owner.transform.position, SeparationDistance);

            // 上昇検出範囲
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Owner.transform.position, Owner.transform.position + Vector3.up * AscentDetectionRange);

            // 距離維持範囲
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new(Owner.transform.position.x, 0, Owner.transform.position.z), MinDistance);

            // 最大高度の表示
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new(Owner.transform.position.x, MaxHeight, Owner.transform.position.z),
                new(Owner.transform.position.x + 5, MaxHeight, Owner.transform.position.z)); // 短いラインで高さを示す
            Gizmos.DrawLine(new(Owner.transform.position.x, MaxHeight, Owner.transform.position.z),
                new(Owner.transform.position.x, MaxHeight, Owner.transform.position.z + 5));

            // 回避方向の可視化（シアン色の円弧）
            Gizmos.color = Color.cyan;
            // Gizmos.DrawWireArc(Owner.transform.position, Vector3.forward, Vector3.up, AscentAvoidanceAngle, AscentDetectionRange);
            // Gizmos.DrawWireArc(Owner.transform.position, Vector3.back, Vector3.up, -AscentAvoidanceAngle, AscentDetectionRange);

            // 地面との距離を可視化（緑色の線）
            Gizmos.color = Color.green;
            var rayOrigin = Owner.transform.position + Vector3.down;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * DetectionRange);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            GizmoManager.Instance.Unregister(this);
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Player = null;
            MinDistance = 7f;
            GizmoManager.Instance.Register(this);
        }

        protected override void Start()
        {
            rb = Owner.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbodyがアタッチされていません: " + Owner.name);
            }

            // TODO
            // obstacleMask の初期化（必要に応じて設定）
            // 例: obstacleMask = LayerMask.GetMask("Obstacles");
            // もしくは、インスペクターから設定可能にするために [SerializeField] を使用
            obstacleMask = LayerMask.GetMask("Default");
        }

        protected override void FixedUpdate()
        {
            if (rb == null || Player == null)
            {
                return;
            }

            var seekForce = Seek(Player.position);
            var avoidForce = ObstacleAvoidance();
            var flockForce = Flocking();
            var ascent = Ascent(); // 上昇力
            var maintainDistance = MaintainDistance(); // 距離維持力

            // 各力にウェイトをかける
            var totalForce = seekForce + avoidForce + flockForce + ascent + maintainDistance;

            // 最大力を制限
            if (totalForce.magnitude > MaxForce)
            {
                totalForce = totalForce.normalized * MaxForce;
            }

            rb.AddForce(totalForce);

            // 速度を制限
            if (rb.velocity.magnitude > MaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * MaxSpeed;
            }

            // TODO
            // playerの方に回転
            var targetRotation = Quaternion.LookRotation(Player.position - Owner.transform.position);
            Owner.transform.rotation =
                Quaternion.Slerp(Owner.transform.rotation, targetRotation, Time.deltaTime * 2.0f);
        }

        protected override bool CheckIfEnd() => false;

        // 追尾（Seek）動作
        private Vector3 Seek(Vector3 targetPosition)
        {
            var desired = (targetPosition - Owner.transform.position).normalized * MoveSpeed;
            var steer = desired - rb.velocity;
            return Vector3.ClampMagnitude(steer, MaxForce);
        }

        // 障害物回避（Obstacle Avoidance）動作
        private Vector3 ObstacleAvoidance()
        {
            RaycastHit hit;
            var avoidance = Vector3.zero;

            // 前方にレイを飛ばして障害物を検出
            if (Physics.Raycast(Owner.transform.position, rb.velocity.normalized, out hit, DetectionRange,
                    obstacleMask))
            {
                // 障害物の法線方向に力を加えて回避
                avoidance = Vector3.Reflect(rb.velocity.normalized, hit.normal) * AvoidanceForce;
            }

            return avoidance;
        }

        // 群れ行動（Flocking）動作
        private Vector3 Flocking()
        {
            var neighbors = GetNeighbors();

            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            var alignment = Align(neighbors);
            var cohesion = Cohere(neighbors);
            var separation = Separate(neighbors);

            // ウェイトを調整
            alignment *= 1.0f;
            cohesion *= 1.0f;
            separation *= 1.5f;

            return alignment + cohesion + separation;
        }

        // 距離維持（Maintain Distance）動作
        private Vector3 MaintainDistance()
        {
            var maintainForce = Vector3.zero;

            // プレイヤーとの水平距離を計算（Y軸を無視）
            var ownerPositionXZ = new Vector3(Owner.transform.position.x, 0, Owner.transform.position.z);
            var playerPositionXZ = new Vector3(Player.position.x, 0, Player.position.z);
            var distanceToPlayer = Vector3.Distance(ownerPositionXZ, playerPositionXZ);

            if (distanceToPlayer < MinDistance)
            {
                // プレイヤーから離れる方向を計算（水平面のみ）
                var fleeDirection = (ownerPositionXZ - playerPositionXZ).normalized;

                // 離れる力を計算（距離が近いほど強くなる）
                maintainForce = fleeDirection * FleeForce * (MinDistance - distanceToPlayer) / MinDistance;
            }

            return maintainForce;
        }

        // 周囲の隣人を取得
        private List<Transform> GetNeighbors()
        {
            var neighbors = new List<Transform>();
            var hits = new Collider[10];
            // TODO: drone 以外の敵を除外する
            //  LayerMask.GetMask("Enemy") などで指定
            var numHits = Physics.OverlapSphereNonAlloc(Owner.transform.position, NeighborDistance, hits);

            for (var i = 0; i < numHits; i++)
            {
                var hit = hits[i];
                if (hit == null)
                {
                    continue;
                }

                var enemyRef = hit.GetComponent<IEnemy>();
                if (hit.transform != Owner.transform && enemyRef != null && enemyRef.EnemyType == EnemyType.Drone)
                {
                    neighbors.Add(hit.transform);
                }
            }

            return neighbors;
        }

        // アラインメント（他の敵と同じ方向に進む）
        private Vector3 Align(List<Transform> neighbors)
        {
            var averageVelocity = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                var neighborRb = neighbor.GetComponent<Rigidbody>();
                if (neighborRb != null)
                {
                    averageVelocity += neighborRb.velocity;
                }
            }

            averageVelocity /= neighbors.Count;
            var steer = averageVelocity.normalized * MoveSpeed - rb.velocity;
            return Vector3.ClampMagnitude(steer, MaxForce);
        }

        // コヒージョン（群れの中心に向かう）
        private Vector3 Cohere(List<Transform> neighbors)
        {
            var centerOfMass = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                centerOfMass += neighbor.position;
            }

            centerOfMass /= neighbors.Count;

            return Seek(centerOfMass);
        }

        // セパレーション（他の敵から離れる）
        private Vector3 Separate(List<Transform> neighbors)
        {
            var separation = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                var distance = Vector3.Distance(Owner.transform.position, neighbor.position);
                if (distance < SeparationDistance && distance > 0)
                {
                    separation += (Owner.transform.position - neighbor.position).normalized / distance;
                }
            }

            separation = separation.normalized * MoveSpeed - rb.velocity;
            return Vector3.ClampMagnitude(separation, MaxForce);
        }

        // 上昇（Ascent）動作
        private Vector3 Ascent()
        {
            var ascent = Vector3.zero;

            // 現在の高さを計算（地面からの高さ）
            var currentHeight = Owner.transform.GetGroundDistance(MaxHeight);

            // 天井までの距離
            var distanceToCeiling = Owner.transform.GetDirectionDistance(Vector3.up, MaxHeight);

            if (currentHeight > TargetHeight || distanceToCeiling > MaxHeight)
            {
                return ascent;
            }

            // 上方向にレイキャストを飛ばして障害物を検出
            if (!Physics.Raycast(Owner.transform.position, Vector3.up, out var hit, AscentDetectionRange,
                    obstacleMask))
            {
                // 障害物がない場合、上方向に力を加える
                ascent = Vector3.up * AscentForce;
            }
            else
            {
                // 障害物がある場合、回避動作を考慮（必要に応じて追加）
                ascent = AvoidAscentObstacle(hit);
            }

            return ascent;
        }

        private Vector3 AvoidAscentObstacle(RaycastHit hit)
        {
            // 障害物の法線を取得
            var hitNormal = hit.normal;

            // 回避方向を計算（法線に基づき左右どちらかに回避）
            // ここでは法線を基に反射方向を計算し、その一部を使用
            var reflection = Vector3.Reflect(Vector3.up, hitNormal);

            // 回避角度を適用
            var avoidanceDirection = Vector3
                .RotateTowards(Vector3.up, reflection, Mathf.Deg2Rad * AscentAvoidanceAngle, 0f).normalized;

            // 回避力を適用
            var avoidanceForceVector = avoidanceDirection * AscentAvoidanceForce;

            return avoidanceForceVector;
        }
    }
}