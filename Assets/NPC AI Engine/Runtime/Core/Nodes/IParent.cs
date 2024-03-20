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
        /// Cached state of the parent
        /// </summary>
        public bool IsCached { get; set; }

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
        public void OnBackPropagate(NodeStatus status, INode sender);
    }

    public static class ParentExtensions
    {
        public static void StartBackPropagation<T>(this IParent thisParent, NodeStatus status, T parent)
            where T : IParent
        {
            thisParent.IsCached = false;
#if LOG_STATES
            Debug.Log("BP from: " + thisParent.GetType().Name + " To: " + parent.GetType().Name);
#endif
            parent.OnBackPropagate(status, thisParent);
            thisParent.Context.OnBackpropagateNotify(thisParent, parent);
        }

        public static Position GetPosition(this IParent parent)
        {
            var pos = new Position();
            pos.inputId = parent.Parent == null ? 0 : parent.Parent.Id;
            pos.outputIds = new();
            for(int i = 0; i < parent.ChildCount; i++)
            {
                int id = 0;
                var child = parent.GetChild(i);
                if(child != null)
                    id = child.Id;
                pos.outputIds.Add(id);
            }
            return pos;
        }
    }
}
