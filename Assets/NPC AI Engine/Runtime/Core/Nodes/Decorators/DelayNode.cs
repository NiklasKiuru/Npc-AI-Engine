using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [EditorNode("Delayes the execution of its child by defined time limit in seconds")]
    public class DelayNode : DecoratorNode
    {
        [SerializeField] private float _delay;

        private float _elapsedTime;

        public DelayNode(int id) : base(id) { }
        
        protected DelayNode(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            var newNode = new DelayNode(Id, Position);
            newNode._delay = _delay;
            return newNode;
        }

        public override void OnBackPropagate(NodeStatus status, INode sender)
        {
            OnInit();
            this.StartBackPropagation(status, Parent);
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            _elapsedTime = 0;
        }

        protected override NodeStatus Tick()
        {
            if(_elapsedTime > _delay)
            {
                var subStatus = Child.Process();
                if (IsCached)
                    this.StartBackPropagation(subStatus, Parent);
                else
                    return subStatus;
            }
            else
            {
                Context.CacheNode(this);
                _elapsedTime += Context.DeltaTime;
                return NodeStatus.Cached;
            }
            return NodeStatus.Succes;
        }
    }

}
