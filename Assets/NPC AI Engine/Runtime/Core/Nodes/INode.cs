using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public interface INode
    {   
        /// <summary>
        /// Processes the node and all its subnodes
        /// </summary>
        /// <returns></returns>
        NodeStatus Process();
        void Build(BehaviourTree tree);
    }
}

