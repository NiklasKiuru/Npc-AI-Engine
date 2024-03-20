using System.Collections.Generic;
using Aikom.AIEngine.Utils;

namespace Aikom.AIEngine
{
    public class RandomSelector : CompositeNode
    {
        private IList<int> _randomIndecies;

        public RandomSelector(int id) : base(id)
        {
        }

        protected RandomSelector(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            return new RandomSelector(Id, Position);
        }

        protected override void OnInit()
        {   
            base.OnInit();
            if(_randomIndecies == null)
            {
                _randomIndecies = new List<int>();
                for(int i = 0; i < ChildCount; i++)
                {
                    _randomIndecies.Add(i);
                }
            }
            _randomIndecies.Suffle();
        }

        protected override NodeStatus ProcessChild(int listIndex)
        {
            return Children[_randomIndecies[listIndex]].Process();
        }

        protected override void OnBuild()
        {
            base.OnBuild();
            BreakStatus = NodeStatus.Succes;
        }
    }
}

