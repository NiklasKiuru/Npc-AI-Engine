using System;
using System.Collections.Generic;
using UnityEngine;
using Aikom.AIEngine.Utils;

namespace Aikom.AIEngine
{
    public class BehaviourTree : IContext
    {
        private Dictionary<string, object> _localVariables = new Dictionary<string, object>();
        private Dictionary<string, object> _globalVariables = new Dictionary<string, object>();
        private Dictionary<string, NodeBase> _nodes = new Dictionary<string, NodeBase>();
        private GameObject _target;

        private bool _active = false;
        private Root _entry;
        private Switch<List<INode>> _switch;

        public GameObject Target { get { return _target; } }

        public BehaviourTree()
        {
            _switch = new Switch<List<INode>>(new List<INode>(), new List<INode>());
        }

        public void Initialize(Root root)
        {
            _entry = root;
            _switch.GetActive().Add(root);
            _active = true;
        }

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


        internal void Stop()
        {   
            _active = false;
            Debug.Log("Stopped tree execution");
        }

        internal void CacheNode(NodeBase node)
        {
            _switch.GetInActive().Add(node);
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
                return (T)value;
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
        /// Global look up for all nodes existing in this tree
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NodeBase GetNode(string name)
        {
            if(_nodes.TryGetValue(name, out var node))
                return node;
            return null;
        }

        public object GetLocalVariable(string name)
        {
            throw new NotImplementedException();
        }

        public void SetLocalVariable(string name, object value)
        {
            throw new NotImplementedException();
        }

        public object GetGlobalVariable(string name)
        {
            throw new NotImplementedException();
        }
    }
}

