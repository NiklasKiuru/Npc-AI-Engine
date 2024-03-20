using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class Sequence : CompositeNode
    {
        public Sequence(int id) : base(id) { }

        protected Sequence(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            return new Sequence(Id, Position);
        }

        protected override void OnBuild()
        {   
            base.OnBuild();
            BreakStatus = NodeStatus.Failure;
        }
    }
}

