using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aikom.AIEngine
{
    [Serializable]
    public class SetDestination : LeafNode
    {
        [CacheVariable(true), SerializeField]
        [Tooltip("Where to find the path end point")]
        private CacheVariable _pathLocation;

        private Vector3 _destination;
        private NavMeshAgent _agent;

        public SetDestination(int id) : base(id)
        {
        }

        protected SetDestination(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            var newNode = new SetDestination(Id, Position);
            newNode._pathLocation = _pathLocation;
            return newNode;
        }

        protected override void OnBuild()
        {   
            _agent = Context.Target.GetComponent<NavMeshAgent>();
        }

        protected override void OnInit()
        {
            _destination = Context.GetLocalVariable<Vector3>(_pathLocation.Name);
        }

        protected override NodeStatus Tick()
        {
            if (_agent.SetDestination(_destination))
                return NodeStatus.Succes;
            return NodeStatus.Failure;
        }
    }

}