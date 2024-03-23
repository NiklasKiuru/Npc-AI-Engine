#undef LOG_STATES

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    /// <summary>
    /// Base class of all behaviour nodes
    /// </summary>
    [Serializable]
    public abstract class NodeBase : INode
    {
        [HideInInspector][SerializeField] private NodeDescriptor _desc;
        [HideInInspector][SerializeField] private Position _position;
        [ReadOnly][SerializeField][Tooltip("Context ID of this node")] private int _id;
        private IParent _parent;
        private BehaviourTree _ctx;
        private NodeStatus _status;

        private event Action<INode, NodeStatus> OnTick;
        private event Action<INode> OnInitialize;

        /// <summary>
        /// Parent of this node
        /// </summary>
        public IParent Parent { get { return _parent; } }

        /// <summary>
        /// The tree this node is part of
        /// </summary>
        public BehaviourTree Context { get { return _ctx; } }

        /// <summary>
        /// Unique id of this node in the context it was created in
        /// </summary>
        public int Id { get { return _id; } }

        /// <summary>
        /// Description data of this node
        /// </summary>
        public NodeDescriptor Descriptor 
        { 
            get => _desc; 
            set 
            { 
                _desc = value;
            } 
        }

        /// <summary>
        /// Relative position of this node to other nodes in the tree
        /// </summary>
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

        /// <summary>
        /// Base constructor for a new node
        /// </summary>
        /// <param name="id"></param>
        public NodeBase(int id)
        {
            _id = id;
            _position = new Position() { outputIds = new() };
        }

        /// <summary>
        /// Constructor for cloning the node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
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

        /// <summary>
        /// Saves the window position in editor
        /// </summary>
        /// <param name="position"></param>
        public void SetWindowPosition(Rect position) 
        {
            var desc = _desc;
            desc.Position = position;
            _desc = desc;
        }

        /// <summary>
        /// Sets a custom name for this node
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name) 
        { 
            var desc = _desc;
            desc.DisplayName = name;
            _desc = desc;
        }

        /// <summary>
        /// Downwards propagating process call
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
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

        /// <summary>
        /// Sets the parent of this node
        /// </summary>
        /// <param name="node"></param>
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

        /// <summary>
        /// Builds connections, callbacks and sets context
        /// </summary>
        /// <param name="t"></param>
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

        /// <summary>
        /// Duplicates this node with new context id and voids position data
        /// </summary>
        /// <returns></returns>
        public NodeBase Duplicate(IReadOnlyCollection<NodeBase> context)
        {
            var copy = Clone() as NodeBase;
            copy._id = ContextId.GenerateSimple(context, this);
            copy._position = new Position() { outputIds = new() };
            return copy;
        }

        /// <summary>
        /// Called every time before Tick if this node was not cached or ticked in this process cycle
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// Defines the status of this node. Called every time on downward process propagation
        /// </summary>
        /// <returns></returns>
        protected abstract NodeStatus Tick();

        /// <summary>
        /// Called only once after the tree has built itself
        /// </summary>
        protected abstract void OnBuild();

        /// <summary>
        /// Defines if the node is valid to be processed. Called after all build logic has been completed
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid(out string message);

        /// <summary>
        /// Clones this node. The implimentation should call an instance constructor with both Id and Position to retain relevant connection information
        /// </summary>
        /// <returns></returns>
        public abstract INode Clone();
    }
}

