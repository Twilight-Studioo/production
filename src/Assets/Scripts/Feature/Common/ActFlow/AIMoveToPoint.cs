using DynamicActFlow.Runtime.Core;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.Common.ActFlow
{
    [ActionTag("AIMoveToPoint")]
    public class AIMoveToPoint: FixedUpdatedAction
    {
        [ActionParameter("Point")] private Vector3 Point { get; set; }
        
        [ActionParameter("MoveSpeed")] private float Speed { get; set; }
        
        private NavMeshAgent agent;
        
        public override void OnCreated()
        {
            base.OnCreated();
            Point = Vector3.zero;
            Speed = 1.0f;
        }

        protected override void Start()
        {
            agent = Owner.GetComponent<NavMeshAgent>();
            agent.SetDestination(Point);
            agent.speed = 3.5f * Speed;
        }

        protected override void FixedUpdate()
        {
            
        }

        protected override bool CheckIfEnd() =>
            Vector3.Distance(Owner.transform.position, Point) < 0.1f;
    }
}