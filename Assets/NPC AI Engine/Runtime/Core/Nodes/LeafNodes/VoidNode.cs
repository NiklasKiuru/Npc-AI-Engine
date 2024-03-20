using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
namespace Aikom.AIEngine
{
    [EditorNode("End point node with no action. Always returns succes")]
    public class VoidNode : LeafNode
    {
        public VoidNode(int id) : base(id)
        {
        }

        public VoidNode(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            return new VoidNode(Id, Position);
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            return NodeStatus.Succes;
        }
    }

}
