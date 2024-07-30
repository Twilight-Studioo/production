#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("AIMoveToFollow")]
    public class AIMoveToFollow : FixedUpdatedAction
    {
        private NavMeshAgent agent;

        [ActionParameter("FollowTransform")] private Transform FollowTransform { get; }

        [ActionParameter("FinishDistance")] private float FinishDistance { get; }

        [ActionParameter("MoveSpeed")] private float MoveSpeed { get; }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            agent.SetDestination(FollowTransform.position);
            agent.speed = 3.5f * MoveSpeed;
        }

        protected override void FixedUpdate()
        {
            agent.SetDestination(FollowTransform.position);
        }

        protected override bool CheckIfEnd() =>
            Vector3.Distance(Owner.transform.position, FollowTransform.position) < FinishDistance;
    }
}