using System;
using Object = UnityEngine.Object;
using UnityEngine;

namespace Aikom.AIEngine
{
    public class FindNode : LeafNode
    {
        [SerializeField]
        private bool _findAll;

        [SerializeField]
        private Type _type;

        [SerializeField]
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
