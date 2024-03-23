using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{   
    /// <summary>
    /// Behaviour node interface
    /// </summary>
    public interface INode : IContextIndexable
    {   
        /// <summary>
        /// Processes the node and all its subnodes
        /// </summary>
        /// <returns></returns>
        public NodeStatus Process();

        /// <summary>
        /// Builds connections and references for this node
        /// </summary>
        /// <param name="tree"></param>
        public void Build(BehaviourTree tree);

        /// <summary>
        /// Information used in editor
        /// </summary>
        public NodeDescriptor Descriptor { get; }

        /// <summary>
        /// Parent of this node
        /// </summary>
        public IParent Parent { get; }

        /// <summary>
        /// Execution context of the node
        /// </summary>
        public BehaviourTree Context { get; }
    }
}

