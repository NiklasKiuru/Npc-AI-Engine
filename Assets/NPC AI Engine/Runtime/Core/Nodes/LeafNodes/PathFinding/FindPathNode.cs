using UnityEngine;
using UnityEngine.AI;

namespace Aikom.AIEngine
{
    [EditorNode("Finds a path using unitys NavMeshAgent")]
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
        [Tooltip("The way the path end point is determined")]
        private PathTarget _target;

        [SerializeField]
        [Tooltip("Maximum distance to look for random positions")]
        private float _maxDistance;

        [SerializeField]
        [CacheVariable(true)]
        [Tooltip("Key to set destination")]
        private CacheVariable _pathDestination;

        [SerializeField]
        [CacheVariable(true)]
        [Tooltip("Key to find object if target type is set to Object")]
        private CacheVariable _objectLocation;

        private bool _hasPath;

        public FindPathNode(int id) : base(id) { }
        protected FindPathNode(int id, Position pos) : base(id, pos) { }

        protected override void OnBuild()
        {
            _agent = Context.Target.GetComponent<NavMeshAgent>();
            _path = new NavMeshPath();
        }

        protected override void OnInit()
        {
            _destination = new Vector3();
            if (_target == PathTarget.Random)
                _destination = Context.Target.transform.position.RandomWithinDistance(_maxDistance);
            else
            {
                var obj = Context.GetLocalVariable<GameObject>(_objectLocation.Name);
                if(obj != null)
                    _destination = obj.transform.position;
            }    
            NavMesh.SamplePosition(_destination, out var hit, _maxDistance, -1);
            
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
                Context.SetLocalVariable(_pathDestination.Name, _destination);
                return NodeStatus.Succes;
            }
                
            return NodeStatus.Failure;
        }

        public override INode Clone()
        {
            var newNode = new FindPathNode(Id, Position);
            newNode._target = _target;
            newNode._maxDistance = _maxDistance;
            newNode._pathDestination = _pathDestination;
            newNode._objectLocation = _objectLocation;
            return newNode;
        }
    }

}
