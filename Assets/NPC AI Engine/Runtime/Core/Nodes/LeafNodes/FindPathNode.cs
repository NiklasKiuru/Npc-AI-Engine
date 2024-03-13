using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Aikom.AIEngine
{
    public class FindPathNode : LeafNode
    {   
        public enum PathTarget
        {
            Random,
            Object
        }

        private NavMeshAgent _agent;
        private NavMeshPath _path;
        private Vector3 _destination;

        [ExposedVariable("Target type", "The way the path end point is determined")]
        private PathTarget _target;

        [ExposedVariable("Max distance", "Maximum distance to look for random positions")]
        private float _maxDistance;

        [ExposedVariable("Local variable cache", "Key to find cached target object")]
        private string _cacheLookUp;


        protected override void OnBuild()
        {
            _agent = Context.Target.GetComponent<NavMeshAgent>();
        }

        protected override void OnInit()
        {
            _destination = new Vector3();
            if (_target == PathTarget.Random)
                _destination = Context.Target.transform.position.RandomWithinDistance(_maxDistance);
            else
            {
                var obj = Context.GetLocalVariable<GameObject>(_cacheLookUp);
                if(obj != null)
                    _destination = obj.transform.position;
            }    
            
            NavMesh.SamplePosition(_destination, out var hit, _maxDistance, -1);
            _path = new NavMeshPath();
            _agent.CalculatePath(hit.position, _path);
        }

        protected override NodeStatus Tick()
        {
            if (_agent.pathPending)
                return NodeStatus.Running;
            if (_agent.hasPath)
            {
                Context.SetLocalVariable(_cacheLookUp, _destination);
                return NodeStatus.Succes;
            }
                
            return NodeStatus.Failure;
        }
    }

}
