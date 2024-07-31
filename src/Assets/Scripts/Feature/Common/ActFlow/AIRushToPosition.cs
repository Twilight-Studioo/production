#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using Feature.Common.Constants;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Common.ActFlow
{
    internal delegate void OnHitRushAttack();

    [ActionTag("AIRushToPosition")]
    public class AIRushToPosition : FixedUpdatedAction
    {
        private NavMeshAgent agent;

        [ActionParameter("TargetTransform")] private Transform PlayerTransform { get; set; }

        [ActionParameter("RushSpeed")] private float Speed { get; set; }

        [ActionParameter("OnHitRushAttack")] private OnHitRushAttack OnHitRushAttack { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();
            Speed = 1f;
            PlayerTransform = null;
            OnHitRushAttack = null;
        }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            agent.SetDestination(PlayerTransform.position);
            agent.speed = ParameterEx.MergePlayerAgentSpeed(3.5f * Speed);
        }

        protected override void FixedUpdate()
        {
            agent.SetDestination(PlayerTransform.position);
        }

        protected override bool CheckIfEnd()
        {
            var check = Vector3.Distance(Owner.transform.position, PlayerTransform.position) < 1.5f;
            if (check && OnHitRushAttack != null)
            {
                OnHitRushAttack();
            }

            return check;
        }
    }
}