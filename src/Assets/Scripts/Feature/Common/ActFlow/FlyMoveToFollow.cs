#region

using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace Feature.Common.ActFlow
{
    [ActionTag("FlyMoveToFollow")]
    public class FlyMoveToFollow : FixedUpdatedAction
    {
        private Rigidbody rb;

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
            rb = Owner.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        protected override void FixedUpdate()
        {
            var direction = (FollowTransform.position - Owner.transform.position).normalized;
            rb.velocity = direction * MoveSpeed;
        }

        protected override bool CheckIfEnd() =>
            Vector3.Distance(Owner.transform.position, FollowTransform.position) < FinishDistance;
    }
}