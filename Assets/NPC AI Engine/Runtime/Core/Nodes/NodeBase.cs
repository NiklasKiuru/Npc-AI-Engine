using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime;
using UnityEditor.Compilation;
using UnityEngine;

namespace Aikom.AIEngine
{
    [Serializable]
    public abstract class NodeBase : ScriptableObject, INode
    {
        private IParent _parent;
        private BehaviourTree _ctx;
        private string _name;
        private NodeStatus _status;

        public IParent Parent { get { return _parent; } }
        public BehaviourTree Context { get { return _ctx; } }
        public string Name { get { return _name; } }
        public NodeStatus Status { get { return _status; } private set { _status = value; } }

        protected abstract void OnInit();
        protected abstract NodeStatus Tick();
        public abstract bool IsValid();

        public NodeStatus Process()
        {   
            // No entry on this process cycle
            if(_status == NodeStatus.Undefined)
            {
                _status = NodeStatus.Running;
                OnInit();
            }
            var status = Tick();
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

        public void SetParent(IParent node) => _parent = node;

        protected abstract void OnBuild();

        void INode.Build(BehaviourTree t)
        {
            _ctx = t;
            OnBuild();
        }
    }

    
}

