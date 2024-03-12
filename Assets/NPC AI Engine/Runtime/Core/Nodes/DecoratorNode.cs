using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public abstract class DecoratorNode : NodeBase, IParent
    {
        private NodeBase _child;
        public NodeBase Child { get { return _child; } }

        public int ChildCount => _child == null ? 0 : 1;

        public NodeBase GetChild(int index) => _child;

        public bool AddChild(NodeBase node)
        {
            if(_child == null)
            {
                _child = node;
                return true;
            }
            return false;
        }

        public void SetChild(int index, NodeBase node) => _child = node;

        public void RemoveChild(int index) => _child = null;

        public abstract void OnBackPropagate(NodeStatus status);
    }
}

