using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Aikom.AIEngine
{
    public class RepeatUntilFail : DecoratorNode
    {   
        public override bool IsValid()
        {
            return Child != null;   
        }

        public override void OnBackPropagate(NodeStatus status)
        {
        }

        protected override void OnBuild()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit()
        {
            throw new System.NotImplementedException();
        }

        protected override NodeStatus Tick()
        {
            return NodeStatus.Undefined;
        }


    }

}
