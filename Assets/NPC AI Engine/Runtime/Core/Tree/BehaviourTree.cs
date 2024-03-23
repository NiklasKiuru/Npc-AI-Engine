using System;
using System.Collections.Generic;
using UnityEngine;
using Aikom.AIEngine.Utils;

namespace Aikom.AIEngine
{
    public class BehaviourTree : IContext
    {
        private Dictionary<string, object> _localVariables;
        private Dictionary<string, object> _globalVariables;
        private Dictionary<int, INode> _nodes;
        private GameObject _target;

        private bool _active = false;
        private Root _root;
        private Switch<List<IParent>> _switch;
        private IExecutionContext _executor;

        /// <summary>
        /// The gameobject this tree is attached to
        /// </summary>
        public GameObject Target { get { return _target; } }

        internal event Action<INode, NodeStatus> OnTick;
        internal event Action<INode> OnInitialize;

        /// <summary>
        /// Called when any node tick has happened
        /// </summary>
        public Action<INode, NodeStatus> OnTickCallback { get { return OnTick; } set { OnTick = value; } }

        /// <summary>
        /// Called when any node has been initialized on its process cycle
        /// </summary>
        public Action<INode> OnInitializeCallback { get { return OnInitialize; } }

        /// <summary>
        /// Called when a node back propagates
        /// </summary>
        public Action<INode, IParent> OnBackpropagateCallback { get; set; }

        /// <summary>
        /// Is this tree actively processing nodes?
        /// </summary>
        public bool IsActive { get { return _active; } }

        /// <summary>
        /// The time interval used in executor
        /// </summary>
        public float DeltaTime { get { return _executor.TickDelay; } }

        /// <summary>
        /// Constructor to create a tree from asset file
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="obj"></param>
        public BehaviourTree(TreeAsset asset, GameObject obj, IExecutionContext executor)
        {   
            _executor = executor;
            _root = asset.Root.Clone() as Root;
            _target = obj;
            _localVariables = new Dictionary<string, object>() { { "", null } };
            foreach(var variable in asset.LocalVariables)
            {   
                _localVariables.TryAdd(variable, default);
            }
            _nodes = new Dictionary<int, INode>
            {
                { -1, _root }
            };
            for (int i = 0; i < asset.Count; i++)
            {
                var node = asset[i];
                if (node.IsConnected(asset))
                {
                    _nodes.Add(node.Id, node.Clone());
                }
            }
        }

        /// <summary>
        /// Initializes the tree
        /// </summary>
        public void Initialize()
        {
            _switch = new Switch<List<IParent>>(new List<IParent>(), new List<IParent>());
            _switch.GetActive().Add(_root);
            _active = true;
            (_root as INode).Build(this);
            foreach (var node in _nodes)
                node.Value.Build(this);
            foreach(var node in _nodes)
            {
                if (!(node.Value as NodeBase).IsValid(out string message))
                {
                    _active = false;
                    Debug.LogError($"Process node: { node.Value.GetType().Name } failed to validate. " +
                        $"Debug id: {node.Value.Id}" +
                        $"Error: {message}" +
                        $"GameObject: {Target}");
                }  
            }
        }

        /// <summary>
        /// Notifies listeners when a tick happens
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        internal void OnTickNotify(INode node, NodeStatus status) 
            => OnTickCallback?.Invoke(node, status);

        /// <summary>
        /// Notifies listeners when back propagation occures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        internal void OnBackpropagateNotify(INode sender, IParent reciever) 
            => OnBackpropagateCallback?.Invoke(sender, reciever);

        // **NOTE** This should propably be public
        /// <summary>
        /// Processes all active branches of the tree
        /// </summary>
        internal void Process()
        {
            if (_active)
            {
                foreach (var node in _switch.GetActive())
                {
                    node.Process();
                }
                _switch.GetActive().Clear();
                _switch.Flip();
            }
        }

        /// <summary>
        /// Gets a node from saved hashmap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        internal T GetNode<T>(int id) where T : INode
        {
            if(id == 0)
                return default;
            if (_nodes.TryGetValue(id, out var node))
                return (T)node;
            return default;
        }

        /// <summary>
        /// Stops the tree execution
        /// </summary>
        internal void Stop()
        {   
            _active = false;
        }

        /// <summary>
        /// Cahces a node for next execution cycle
        /// </summary>
        /// <param name="node"></param>
        internal void CacheNode(IParent node)
        {
            node.IsCached = true;
            _switch.GetInActive().Add(node);
#if UNITY_EDITOR
            var inActiveList = _switch.GetInActive();
            if(inActiveList.Count > _nodes.Count && _active)
            {
                _active = false;
                var dic = new Dictionary<IParent, int>();
                Debug.LogError("Cache overload detected. Shutting down processing.");
                for(int i = 0; i < inActiveList.Count; i++)
                {
                    if (!dic.TryAdd(inActiveList[i], 1))
                    {
                        dic[inActiveList[i]] += 1;
                    }
                }
                foreach(var kvp in dic)
                {
                    Debug.LogWarning($"Node: {kvp.Key.GetType()}, cache count: {kvp.Value}");
                }
            }
#endif
        }
        
        /// <summary>
        /// Gets a local shared variable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetLocalVariable<T>(string name)
        {
            if (_localVariables.TryGetValue(name, out object value))
            {
                if(value == null)
                    return default;
                return (T)value;
            }
                
            return default;
        }

        /// <summary>
        /// Gets a global variable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetGlobalVariable<T>(string name)
        {
            if( _globalVariables.TryGetValue(name,out object value)) 
                return (T)value;
            return default;
        }

        /// <summary>
        /// Gets a local variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetLocalVariable(string name)
        {
            if(_localVariables.TryGetValue(name, out var obj))
                return obj;
            return default;
        }

        /// <summary>
        /// Sets a local variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetLocalVariable(string name, object value)
        {
            if (_localVariables.ContainsKey(name))
                _localVariables[name] = value;
        }

        /// <summary>
        /// Gets a global variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object GetGlobalVariable(string name)
        {
            throw new NotImplementedException();
        }
    }
}

