#undef LOG_STATES

using System;
using UnityEngine;

namespace Aikom.AIEngine
{
    [Serializable]
    public abstract class NodeBase : INode
    {
        [HideInInspector][SerializeField] private NodeDescriptor _desc;
        [HideInInspector][SerializeField] private Position _position;
        [ReadOnly][SerializeField][Tooltip("Context ID of this node")] private int _id;
        private IParent _parent;
        private BehaviourTree _ctx;
        private NodeStatus _status;

        public event Action<INode, NodeStatus> OnTick;
        public event Action<INode> OnInitialize;

        public IParent Parent { get { return _parent; } }
        public BehaviourTree Context { get { return _ctx; } }
        public int Id { get { return _id; } }
        public NodeDescriptor Descriptor 
        { 
            get => _desc; 
            set 
            { 
                _desc = value;
            } 
        }
        public Position Position 
        { 
            get 
            {
                if (_position.outputIds == null)
                {
                    var pos = _position;
                    pos.outputIds = new();
                    _position = pos;
                }
                return _position; 
            } 
        }

        public NodeBase(int id)
        {
            _id = id;
            _position = new Position() { inputId = 0, outputIds = new() };
        }

        protected NodeBase(int id, Position position)
        {
            _id = id;
            _position = new Position() { inputId = position.inputId, outputIds = new() };
            for(int i = 0; i < position.outputIds.Count; i++)
            {
                _position.outputIds.Add(position.outputIds[i]);
            }
        }

        /// <summary>
        /// Updates serializeable position data of this node
        /// </summary>
        public void UpdatePositionData(Position pos)
        {   
            _position = pos;
        }

        /// <summary>
        /// Checks if the node is connected to a root object of this asset
        /// </summary>
        /// <returns></returns>
        public bool IsConnected(TreeAsset asset)
        {
            if (_position.IsAttached)
            {
                NodeBase parent = asset.GetNode(_position.inputId);
                while (parent != null)
                {
                    if (parent is Root)
                        return true;
                    parent = asset.GetNode(parent.Position.inputId);
                }
            }

            return false;
        }

        public void SetComment(string comment) 
        { 
            var desc = _desc;
            desc.UserComment = comment;
            _desc = desc;
        } 
        public void SetWindowPosition(Rect position) 
        {
            var desc = _desc;
            desc.Position = position;
            _desc = desc;
        }
        public void SetName(string name) 
        { 
            var desc = _desc;
            desc.DisplayName = name;
            _desc = desc;
        }


        protected abstract void OnInit();
        protected abstract NodeStatus Tick();
        public abstract bool IsValid();

        public NodeStatus Process()
        {
#if LOG_STATES
            Debug.Log("Processing: " + GetType().Name);
#endif
            // No entry on this process cycle
            if(_status == NodeStatus.Undefined)
            {
                _status = NodeStatus.Running;
#if LOG_STATES
                Debug.Log(this.GetType().Name + " Initialized");
#endif
                OnInit();
                OnInitialize?.Invoke(this);
            }
            var status = Tick();
            OnTick?.Invoke(this, status);
            if (status == NodeStatus.Undefined)
                throw new System.Exception("Undefined return status");

            // Exit condition where the status is set back into its original undefined state
            // The method still returns the results from the tick function instead of the state
            if (status == NodeStatus.Succes || status == NodeStatus.Failure)
            {   
                _status = NodeStatus.Undefined;
                return status;
            }
            else
                _status = status;
            return _status;
        }

        public void SetParent(IParent node)
        {
            _parent = node;
            var pos = _position;
            if (_parent != null)
                pos.inputId = node.Id;
            else
                pos.inputId = 0;
            _position = pos;
        }

        protected abstract void OnBuild();

        void INode.Build(BehaviourTree t)
        {
            _ctx = t;
            _status = NodeStatus.Undefined;
            _parent = t.GetNode<IParent>(_position.inputId);
            if( _parent != null)
            {
                var parentBase = _parent as NodeBase;
                for (int i = 0; i < parentBase.Position.outputIds.Count; i++)
                {
                    var childId = parentBase.Position.outputIds[i];
                    if (childId == _id)
                    {
                        _parent.SetChild(i, this);
                    }
                }
            }
            OnTick = t.OnTickNotify;

            OnBuild();
        }

        public abstract INode Clone();
        
    }

    
}

