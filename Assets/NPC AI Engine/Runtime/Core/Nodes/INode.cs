using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public interface INode : IContextIndexable
    {   
        /// <summary>
        /// Processes the node and all its subnodes
        /// </summary>
        /// <returns></returns>
        NodeStatus Process();

        /// <summary>
        /// Builds connections and references for this node
        /// </summary>
        /// <param name="tree"></param>
        void Build(BehaviourTree tree);

        /// <summary>
        /// Information used in editor
        /// </summary>
        NodeDescriptor Descriptor { get; }

        /// <summary>
        /// Parent of this node
        /// </summary>
        IParent Parent { get; }

        /// <summary>
        /// Execution context of the node
        /// </summary>
        public BehaviourTree Context { get; }
    }
}

