using UnityEngine;
using System;

namespace Aikom.AIEngine
{
    // **NOTE** This should propably be moved to editor assembly

    /// <summary>
    /// Holds descriptive information for a node. Mainly used for editor
    /// </summary>
    [Serializable]
    public struct NodeDescriptor
    {
        private static Vector2 s_defaultSize = new Vector2(200, 150);
        private static Vector2Int s_compositeConfig = new Vector2Int(-1, 2);
        private static Vector2Int s_leafConfig = new Vector2Int(0, 0);
        private static Vector2Int s_decoratorConfig = new Vector2Int(1, 1);

        [SerializeField] private int _maxChildren;
        [SerializeField] private int _minChildren;
        [SerializeField] private string _userComment;
        [SerializeField] private Rect _window;
        [SerializeField] private string _name;

        /// <summary>
        /// Maximum allowed children for this node. -1 = No limit
        /// </summary>
        public int MaxChildren { get => _maxChildren; }

        /// <summary>
        /// Minimum amount of children this node has to have
        /// </summary>
        public int MinChildren { get => _minChildren; }

        /// <summary>
        /// Window size used in UI
        /// </summary>
        public Vector2 DefaultWindowSize { get => _window.size; }

        /// <summary>
        /// User defined comment for this node
        /// </summary>
        public string UserComment { get => _userComment; set => _userComment = value; }

        /// <summary>
        /// Used window rect in editor
        /// </summary>
        public Rect Position { get => _window; set => _window = value; }

        /// <summary>
        /// Name displayed in the editor
        /// </summary>
        public string DisplayName { get => _name; set => _name = value; }

        public NodeDescriptor(int maxChildren, int minChildren)
        {
            _maxChildren = maxChildren;
            _minChildren = minChildren;
            _window = new Rect(Vector2.zero, s_defaultSize);
            _userComment = string.Empty;
            _name = string.Empty;
        }

        /// <summary>
        /// Gets the default child configuration for the node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Vector2Int DefaultChildConfiguration<T>(T node) where T : INode
        {
            Vector2Int conf = new();
            if (node is CompositeNode)
                conf = s_compositeConfig;
            if (node is LeafNode)
                conf = s_leafConfig;
            if (node is DecoratorNode)
                conf = s_decoratorConfig;
            return conf;
        }

        /// <summary>
        /// Gets a prittified name for the node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetDefaultPrettyName<T>(T node) where T : INode
        {
           return node.GetType().Name.Replace("Node", "");
        }
    }
}
