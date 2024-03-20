using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    [EditorNode("Logs the message to the console and returns succes")]
    public class DebugNode : LeafNode
    {
        [SerializeField, ExposedVariable("Message")] 
        private string _message;

        public DebugNode(int id) : base(id)
        {
        }

        public DebugNode(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            return new DebugNode(Id, Position);
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            Debug.Log(_message);
            return NodeStatus.Succes;
        }
    }

}
