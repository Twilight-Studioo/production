#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Feature
{
    [ActionTag("MoveLoop")]
    public sealed class MoveAction : FixedUpdatedAction
    {
        private int currentLoop;
        private bool movingTowardsTarget = true;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        [ActionParameter("LoopCount", 1)] private int LoopCount { get; set; }

        [ActionParameter("Direction")] private Vector3 Direction { get; set; }

        [ActionParameter("Range", 5f)] private float Range { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            LoopCount = 1;
            Range = 5f;
            Direction = Vector3.forward;
        }

        protected override void Start()
        {
            startPosition = Owner.transform.position; // 初期位置を保存
            targetPosition = startPosition + Direction.normalized * Range; // 目標位置を計算
        }

        protected override void FixedUpdate()
        {
            if (currentLoop < LoopCount)
            {
                var step = 5f * Time.fixedDeltaTime; // 移動距離を計算
                var target = movingTowardsTarget ? targetPosition : startPosition;

                // 現在位置から目標位置に向かって移動
                Owner.transform.position = Vector3.MoveTowards(Owner.transform.position, target, step);

                // 目標に到達した場合、方向を切り替え
                if (Vector3.Distance(Owner.transform.position, target) < 0.01f)
                {
                    movingTowardsTarget = !movingTowardsTarget; // 移動方向を反転
                    if (movingTowardsTarget)
                    {
                        currentLoop++; // 往復が完了したと見なす
                        movingTowardsTarget = true;
                    }
                }
            }
        }

        protected override bool CheckIfEnd() => currentLoop >= LoopCount;
    }
}