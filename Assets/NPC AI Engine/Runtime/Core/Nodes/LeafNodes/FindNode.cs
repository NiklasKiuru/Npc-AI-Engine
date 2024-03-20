using System;
using Object = UnityEngine.Object;

namespace Aikom.AIEngine
{
    public class FindNode : LeafNode
    {
        [ExposedVariable("Find all", "Should the search be done for the entire hierarchy?")]
        private bool _findAll;

        [ExposedVariable("Unity object type", "Must derive from MonoBehaviour and have an attribute Discoverable")]
        private Type _type;

        [ExposedVariable("Local variable cache")]
        private string _localVariable;

        public FindNode(int id) : base(id)
        {
        }

        protected FindNode(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            var newNode = new FindNode(Id, Position);
            newNode._findAll = _findAll;
            newNode._type = _type;
            newNode._localVariable = _localVariable;
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
            if (_type == null || string.IsNullOrEmpty(_localVariable))
                return NodeStatus.Failure;
            if (_findAll)
            {   
                var objects = Object.FindObjectsOfType(_type);
                if(objects.Length > 0)
                {   
                    Context.SetLocalVariable(_localVariable, objects);
                    return NodeStatus.Succes;
                }
            }

            else
            {
                var uObject = Object.FindObjectOfType(_type);
                if (uObject != null) 
                {
                    Context.SetLocalVariable(_localVariable, uObject);
                    return NodeStatus.Succes;
                }
                
            }

            return NodeStatus.Failure;
        }
    }

}
