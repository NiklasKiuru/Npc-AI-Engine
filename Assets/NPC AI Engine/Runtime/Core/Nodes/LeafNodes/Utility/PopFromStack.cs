using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    [EditorNode("Pops an object from stack and stores it to specified destination")]
    public class PopFromStack : LeafNode
    {
        [SerializeField, Tooltip("Read stack from"), CacheVariable]
        private CacheVariable _readFrom;

        [SerializeField, Tooltip("Write result to this location"), CacheVariable(true)]
        private CacheVariable _writeTo;

        private Stack _stack;

        public PopFromStack(int id) : base(id)
        {
        }

        public PopFromStack(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            var newNode = new PopFromStack(Id, Position);
            newNode._readFrom = _readFrom;
            newNode._writeTo = _writeTo;
            return newNode;
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
            _stack = _readFrom.Space == CacheSpace.Global 
                ? Context.GetGlobalVariable<Stack>(_readFrom.Name) 
                : Context.GetLocalVariable<Stack>(_readFrom.Name);
        }

        protected override NodeStatus Tick()
        {
            if(_stack != null)
            {
                Context.SetLocalVariable(_writeTo.Name, _stack.Pop());
                return NodeStatus.Succes;
            }
            return NodeStatus.Failure;
        }
    }

}
