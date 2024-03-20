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

        [SerializeField]
        [ExposedVariable("Target type", "The way the path end point is determined")]
        private PathTarget _target;

        [SerializeField]
        [ExposedVariable("Max distance", "Maximum distance to look for random positions")]
        private float _maxDistance;

        [SerializeField]
        [CacheVariable(true)]
        [ExposedVariable("Local variable cache", "Key to find cached target object")]
        private CacheVariable _cacheLookUp;

        private bool _hasPath;

        public FindPathNode(int id) : base(id) { }
        protected FindPathNode(int id, Position pos) : base(id, pos) { }

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
                var obj = Context.GetLocalVariable<GameObject>(_cacheLookUp.Name);
                if(obj != null)
                    _destination = obj.transform.position;
            }    
            
            NavMesh.SamplePosition(_destination, out var hit, _maxDistance, -1);
            _path = new NavMeshPath();
            if (!hit.hit)
                _hasPath = false;
            else
                _hasPath = _agent.CalculatePath(hit.position, _path);
        }

        protected override NodeStatus Tick()
        {
            if (!_hasPath)
                return NodeStatus.Failure;
            if (_hasPath)
            {
                Context.SetLocalVariable(_cacheLookUp.Name, _destination);
                return NodeStatus.Succes;
            }
                
            return NodeStatus.Failure;
        }

        public override INode Clone()
        {
            var newNode = new FindPathNode(Id, Position);
            newNode._target = _target;
            newNode._maxDistance = _maxDistance;
            newNode._cacheLookUp = _cacheLookUp;
            return newNode;
        }
    }

}
