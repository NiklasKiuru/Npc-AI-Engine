using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [EditorNode("Processes all nodes each tick until all children return succes")]
    public class ParallelSequence : CompositeNode
    {
        private NodeStatus[] _cachedStates;

        public ParallelSequence(int id) : base(id)
        {
        }

        protected ParallelSequence(int id, Position pos) : base(id, pos) { }

        public override void OnBackPropagate(NodeStatus status, INode sender)
        {   
            int index = 0;
            for(int i = 0; i < _cachedStates.Length; i++)
            {
                if (Children[i].Id == sender.Id)
                    break;
                index++;
            }
            _cachedStates[index] = status;
            if (index == _cachedStates.Length - 1 && status == NodeStatus.Succes)
            {
                var succes = false;
                for(int i = 0; i < _cachedStates.Length; i++)
                {
                    succes &= _cachedStates[i] == NodeStatus.Succes;
                }
                if(succes)
                    this.StartBackPropagation(status, Parent);
            }

        }

        protected override NodeStatus Tick()
        {
            var succes = false;
            for(int i = 0; i < ChildCount; i++)
            {
                NodeStatus subStatus;
                if (_cachedStates[i] != NodeStatus.Cached)
                {
                    subStatus = ProcessChild(i);
                    _cachedStates[i] = subStatus;
                    succes &= subStatus == NodeStatus.Succes;
                }
                else
                    succes = false;
                    
            }
            if (succes)
            {
                if (IsCached)
                    this.StartBackPropagation(NodeStatus.Succes, Parent);
                else
                    return NodeStatus.Succes;
            }
            else
            {
                Context.CacheNode(this);
                return NodeStatus.Cached;
            }
            return NodeStatus.Undefined;
        }

        protected override void OnBuild()
        {
            _cachedStates = new NodeStatus[ChildCount];
        }

        public override INode Clone()
        {
            return new ParallelSequence(Id, Position);
        }
    }

}
