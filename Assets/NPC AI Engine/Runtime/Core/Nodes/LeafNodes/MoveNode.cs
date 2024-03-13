using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    public class MoveNode : LeafNode
    {
        private string _pathLocation;
        private Vector3 _destination;

        protected override void OnBuild()
        {   
        }

        protected override void OnInit()
        {

        }

        protected override NodeStatus Tick()
        {
            throw new System.NotImplementedException();
        }
    }

}