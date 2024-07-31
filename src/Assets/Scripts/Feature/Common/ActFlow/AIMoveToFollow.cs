#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using Feature.Common.Constants;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("AIMoveToFollow")]
    public class AIMoveToFollow : FixedUpdatedAction
    {
        private NavMeshAgent agent;

        [ActionParameter("FollowTransform")] private Transform FollowTransform { get; set; }

        [ActionParameter("FinishDistance")] private float FinishDistance { get; set; }

        [ActionParameter("MoveSpeed")] private float MoveSpeed { get; set; }


        public override void OnCreated()
        {
            base.OnCreated();
            FinishDistance = 1.0f;
            MoveSpeed = 1.0f;
            FollowTransform = null;
        }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            agent.SetDestination(FollowTransform.position);
            agent.speed = ParameterEx.MergePlayerAgentSpeed(3.5f * MoveSpeed);
        }

        protected override void FixedUpdate()
        {
            agent.SetDestination(FollowTransform.position);
        }

        protected override bool CheckIfEnd() =>
            Vector3.Distance(Owner.transform.position, FollowTransform.position) < FinishDistance;
    }
}