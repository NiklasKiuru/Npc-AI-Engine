using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    [EditorNode("Checks if the two objects are equal. To null check just leave the field as blank")]
    public class EqualsNode : LeafNode
    {
        [Tooltip("Object 1 location"), SerializeField, CacheVariable]
        private CacheVariable _object1;

        [Tooltip("Object 1 location"), SerializeField, CacheVariable]
        private CacheVariable _object2;

        public EqualsNode(int id) : base(id) { }
        protected EqualsNode(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            var newNode = new EqualsNode(Id, Position);
            newNode._object1 = _object1;
            newNode._object2 = _object2;
            return newNode;
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            var val1 = Read(_object1.Space, _object1.Name);
            var val2 = Read(_object2.Space, _object2.Name);

            if (val1 != val2)
                return NodeStatus.Failure;
            return NodeStatus.Succes;
        }

        private object Read(CacheSpace space, string name)
        {
            if(space == CacheSpace.Local)
                return Context.GetLocalVariable(name);
            return Context.GetGlobalVariable(name);
        }
    }

}
