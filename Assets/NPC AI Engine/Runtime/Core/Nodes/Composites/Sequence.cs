using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class Sequence : CompositeNode
    {
        protected override void OnBuild()
        {
            BreakStatus = NodeStatus.Failure;
        }
    }
}

