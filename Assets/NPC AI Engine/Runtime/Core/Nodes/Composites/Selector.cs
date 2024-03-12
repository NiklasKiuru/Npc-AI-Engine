using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class Selector : CompositeNode
    {   
        protected override void OnBuild()
        {
            BreakStatus = NodeStatus.Succes;
        }
    }
}
