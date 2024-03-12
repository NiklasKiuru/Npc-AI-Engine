using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Aikom.AIEngine
{   
    /// <summary>
    /// Interface for nodes that can be parent of other nodes
    /// </summary>
    public interface IParent : INode
    {   
        /// <summary>
        /// Number of children this node has
        /// </summary>
        public int ChildCount { get; }

        /// <summary>
        /// Gets the n.th child of this node
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public NodeBase GetChild(int index);

        /// <summary>
        /// Tries to add a child node to this node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool AddChild(NodeBase node);

        /// <summary>
        /// Sets the n.th child of this node
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        public void SetChild(int index, NodeBase node);

        /// <summary>
        /// Removes the n.th child from this node
        /// </summary>
        /// <param name="index"></param>
        public void RemoveChild(int index);

        /// <summary>
        /// Function that provides back propagation functionality. If a node has been cached in the process cycle it must use back propagation
        /// to send its state higher in the hierarchy
        /// </summary>
        /// <param name="status"></param>
        public void OnBackPropagate(NodeStatus status);
    }
}
