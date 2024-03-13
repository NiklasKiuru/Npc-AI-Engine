using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    public class ParallelSequence : CompositeNode
    {
        public override void OnBackPropagate(NodeStatus status)
        {
            base.OnBackPropagate(status);
        }

        protected override NodeStatus Tick()
        {
            return base.Tick();
        }

        protected override void OnBuild()
        {
        }
    }

}
