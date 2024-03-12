using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class Root : NodeBase, IParent
    {
        private NodeBase _child;
        public int ChildCount => _child == null ? 0 : 1;

        public bool AddChild(NodeBase node)
        {   
            if(_child == null)
            {
                _child = node;
                return true;
            }
            return false;
        }

        public NodeBase GetChild(int index) => _child;

        public void OnBackPropagate(NodeStatus status)
        {
            // Notify context and abort tree
            if(status != NodeStatus.Running || status != NodeStatus.Cached)
            {
                Debug.Log("Root exited with status: " + status);
                Context.Stop();
            }
        }

        protected override NodeStatus Tick()
        {   
            var subStatus = _child.Process();
            return subStatus;
        }

        public void RemoveChild(int index) => _child = null;

        public void SetChild(int index, NodeBase node) => _child = node;

        protected override void OnInit()
        {
        }

        public override bool IsValid()
        {
            return _child != null;
        }

        protected override void OnBuild()
        {
        }
    }

}
