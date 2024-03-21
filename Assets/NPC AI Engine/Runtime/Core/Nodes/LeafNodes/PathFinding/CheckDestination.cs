using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Aikom.AIEngine
{
    public class CheckDestination : LeafNode
    {
        private NavMeshAgent _agent;

        public CheckDestination(int id) : base(id)
        {
        }

        protected CheckDestination(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            return new CheckDestination(Id, Position);
        }

        protected override void OnBuild()
        {
            _agent = Context.Target.GetComponent<NavMeshAgent>();
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance &&
                (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0))
                return NodeStatus.Succes;
            return NodeStatus.Running;
        }
    }

}
