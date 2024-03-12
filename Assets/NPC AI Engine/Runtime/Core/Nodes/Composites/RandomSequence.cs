using Aikom.AIEngine;
using Aikom.AIEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class RandomSequence : Sequence
    {
        private IList<int> _randomIndecies;
        protected override void OnInit()
        {
            base.OnInit();
            if (_randomIndecies == null)
            {
                _randomIndecies = new List<int>();
                for (int i = 0; i < ChildCount; i++)
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
    }
}
