using System.Collections;
using System.Linq;
using Core.Utilities;
using Feature.Component.Enemy.SmasherEx.State;
using UnityEngine;
using UnityEngine.AI;
using ObjectFactory = Core.Utilities.ObjectFactory;

namespace Feature.Component.Enemy.SmasherEx
{
    public partial class SmasherAI
    {

        private const float Angle = 45f; // 投げる角度（度）
        private const float MaxFindPlayerDistance = 30f; // プレイヤーを探す距離
        /// <summary>
        /// playerに向かって移動する時の目標距離
        /// ForwardBlowなどで使用
        /// </summary>
        private const float MoveToPlayerTargetDistance = 4f;
        
        /// <summary>
        /// 一定距離を保つ時の目標距離
        /// 落下攻撃などで使用
        /// </summary>
        private const float MoveToKeepDistanceTargetDistance = 10f;
        

    
        private partial Vector3 CalculateVelocity(GameObject mine, Vector3 target, float angle, float velocityMultiplier, bool useGravity)
        {
            var start = mine.transform.position;

            // 平面上の距離
            var direction = target - start;
            var distance = new Vector3(direction.x, 0, direction.z).magnitude;

            // 高低差
            var heightDifference = direction.y;

            // 角度をラジアンに変換
            var angleRad = Mathf.Deg2Rad * angle;

            // 重力補正
            var gravity = Physics.gravity.magnitude * 1.0f; // 必要に応じて倍率を調整

            // 初速度を計算
            var velocitySquared = (gravity * distance * distance) /
                                  (2 * (distance * Mathf.Tan(angleRad) - heightDifference));
            if (velocitySquared < 0)
            {
                Debug.LogWarning($"Invalid velocity calculation: Check target position or angle. {velocitySquared}");
                velocitySquared = 0.1f; // 負の値を回避するための補正
            }

            var velocity = Mathf.Sqrt(velocitySquared);

            // 水平方向と垂直方向の速度成分
            var vy = velocity * Mathf.Sin(angleRad);
            var vxz = velocity * Mathf.Cos(angleRad);

            // 平面方向の単位ベクトル
            var horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

            // 最終速度ベクトル（補正を掛ける
            var result = (horizontalDirection * vxz + Vector3.up * vy) * velocityMultiplier;

            // checked Nan
            result.x = result.x.ClampNan();
            result.y = result.y.ClampNan();
            result.z = result.z.ClampNan();
            return result;
        }

        private void ChargingForwardTick(float chargeSpeed, float rayLength)
        {
    
            var minePosition = spawnedMine.transform.position;
            var toMine = minePosition - transform.position;
            var mineToPlayerDirection = toMine.normalized;
            agent.velocity = mineToPlayerDirection * chargeSpeed;
            var hits = transform.GetBoxCastAll(new Vector3(1f, 1f, 1f), mineToPlayerDirection, rayLength, 10);
            // hitしたobjに突進の力を加える
            foreach (var raycastHit in hits)
            {
                if (raycastHit.transform == null || raycastHit.transform == transform)
                {
                    continue;
                }

                var dir = (raycastHit.transform.position - transform.position).normalized;
                dir.y += 0.7f;
                var hitRb = raycastHit.transform.GetComponent<Rigidbody>();
                if (hitRb != null && hitRb.gameObject != spawnedMine.gameObject)
                {
                    var velocity = CalculateVelocity(hitRb.gameObject, spawnedMine.transform.position, 20f,
                        1.0f);
                    hitRb.velocity = velocity;
                }
            }
        }
        
        /// <summary>
        /// 2点間に指定objが存在するか
        /// </summary>
        private bool ExistsBetweenTwoPoints(Vector3 from, Vector3 to, Transform obj)
        {
            var direction = to - from;
            var distance = direction.magnitude;
            var hits = transform.GetBoxCastAll(new Vector3(1f, 1f, 1f), direction.normalized, distance, 10);
            return hits.Any(hit => hit.transform == obj);
        }
        
        /// <summary>
        /// 指定秒数後にAIを再開する
        /// </summary>
        private IEnumerator DelaySecondsRestartFlow(float delay)
        {
            FlowStop();
            yield return new WaitForSeconds(delay);
            FlowStart();
        }

        /// <summary>
        /// プレイヤーから一定距離or至近距離のどちらかにランダムで目標地点を打つ
        /// 至近距離の場合は、playerに向かって[MoveToPlayerTargetDistance]の距離を保つ
        /// 一定距離の場合は、playerから[KeepDistanceTargetDistance]の距離を保つように指定するが、壁などにより到達できない場合は至近距離になる
        /// </summary>
        private void MoveToPlayerWithAgent()
        {
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            var playerLeaveDirection = (transform.position - player.transform.position).normalized;
            var leaveTargetPosition = player.transform.position + playerLeaveDirection * MoveToKeepDistanceTargetDistance;
            var canKeepDistance = IsDestinationReachable(leaveTargetPosition);
            var isMoveToLeave = canKeepDistance && Random.Range(0, 2) == 0;
            if (isMoveToLeave)
            {
                // 一定距離を保つように動く
                agent.SetDestination(leaveTargetPosition);
                state = MovementState.MoveToLeavePlayerWithAgent;
                destinationReached = false;
                Debug.Log("一定距離を保つように動く");
            }
            else
            {
                // 至近距離に移動
                var nearPlayerPosition = player.transform.position + playerLeaveDirection * MoveToPlayerTargetDistance;
                agent.SetDestination(nearPlayerPosition);
                state = MovementState.MoveToNearPlayerWithAgent;
                destinationReached = false;
                Debug.Log("至近距離に移動");
            }
        }
        
        /// <summary>
        /// NavMeshAgentが指定地点に到達可能か
        /// </summary>
        private bool IsDestinationReachable(Vector3 destination)
        {
            var path = new NavMeshPath();
            var pathFound = NavMesh.CalculatePath(agent.transform.position, destination, NavMesh.AllAreas, path);

            if (!pathFound)
                return false;

            return path.status == NavMeshPathStatus.PathComplete;
        }
        
        /// <summary>
        /// playerとの距離を取得
        /// </summary>
        private float ToPlayerDistance()
        {
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            return Vector3.Distance(transform.position, player.transform.position);
        }
    }
    
    internal static class SmasherAIExtension
    {
        public static float ClampNan(this float value)
        {
            return float.IsNaN(value) ? 0 : value;
        }
    }
}