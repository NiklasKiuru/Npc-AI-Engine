using UnityEngine;
using System;

namespace Aikom.AIEngine
{
    [Serializable]
    public struct NodeDescriptor
    {
        private static Vector2 s_defaultSize = new Vector2(200, 150);
        private static Vector2Int s_compositeConfig = new Vector2Int(-1, 2);
        private static Vector2Int s_leafConfig = new Vector2Int(0, 0);
        private static Vector2Int s_decoratorConfig = new Vector2Int(1, 1);

        [SerializeField] private string _description;
        [SerializeField] private int _maxChildren;
        [SerializeField] private int _minChildren;
        [SerializeField] private string _userComment;
        [SerializeField] private Rect _window;
        [SerializeField] private string _name;

        /// <summary>
        /// Description of the node
        /// </summary>
        public string Description { get => _description; }

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
        public string UserComment { get => _userComment; set => _description = value; }

        /// <summary>
        /// Used window rect in editor
        /// </summary>
        public Rect Position { get => _window; set => _window = value; }

        /// <summary>
        /// Name displayed in the editor
        /// </summary>
        public string DisplayName { get => _name; set => _name = value; }

        public NodeDescriptor(string desc, int maxChildren, int minChildren)
        {
            _description = desc;
            _maxChildren = maxChildren;
            _minChildren = minChildren;
            _window = new Rect(Vector2.zero, s_defaultSize);
            _userComment = string.Empty;
            _name = string.Empty;
        }

        public static Vector2Int DefaultChildConfiguration<T>(T node)
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

        public static string GetDefaultPrettyName<T>(T node)
        {
           return node.GetType().Name.Replace("Node", "");
        }
    }

    internal static class Descriptors
    {
        private static Vector2 _defaultSize = new Vector2(200, 150);
        private const string _rootDesc = "Root node of the tree";
        private const string _selectorDesc = "Selects children from left to right until a child succeeds or all of them fail";
        private const string _randSelectorDesc = "Selects children in random order until a child succeeds or all of them fail";
        private const string _seqDesc = "Executes children from left to right until either one fails or all succeed";
        private const string _randSeqDesc = "Executes children in random order until either one fails or all succeed";
        private const string _inverterDesc = "Inverts the result of its child";
        private const string _seekDesc = "Physics based search query. Returns found colliders in order from closest to farthest";
        private const string _findDesc = "Finds object references from scene hierarchy";


    }

}
