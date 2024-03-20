using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class Selector : CompositeNode
    {
        public Selector(int id) : base(id)
        {
        }

        protected Selector(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            return new Selector(Id, Position);
        }

        protected override void OnBuild()
        {   
            base.OnBuild();
            BreakStatus = NodeStatus.Succes;
        }
    }
}
