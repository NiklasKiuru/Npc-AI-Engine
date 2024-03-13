using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Aikom.AIEngine
{
    [Serializable]
    public struct NodeDescriptor
    {
        private static Vector2 s_defaultSize = new Vector2(200, 150);

        /// <summary>
        /// Description of the node
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Maximum allowed children for this node. -1 = No limit
        /// </summary>
        public int MaxChildren { get; private set; }

        /// <summary>
        /// Minimum amount of children this node has to have
        /// </summary>
        public int MinChildren { get; private set; }

        /// <summary>
        /// Window size used in UI
        /// </summary>
        public Vector2 DefaultWindowSize { get; private set; }

        /// <summary>
        /// User defined comment for this node
        /// </summary>
        public string UserComment { get; private set; }

        /// <summary>
        /// Used window rect in editor
        /// </summary>
        public Rect Position { get; private set; }

        /// <summary>
        /// Name displayed in the editor
        /// </summary>
        public string DisplayName { get; private set; }

        public int Id { get; private set; }

        public NodeDescriptor(string desc, int maxChildren, int minChildren, Vector2 window)
        {
            Description = desc;
            MaxChildren = maxChildren;
            MinChildren = minChildren;
            if (window == Vector2.zero)
                DefaultWindowSize = s_defaultSize;
            else
                DefaultWindowSize = window;
            UserComment = string.Empty;
            Position = new Rect();
            DisplayName = string.Empty;
            Id = -1;
        }

        public void SetComment(string comment) => UserComment = comment;
        public void SetPosition(Rect position) => Position = position;
        public void SetName(string name) => DisplayName = name;

        private static int GetContextId<T>(T node, TreeAsset asset)
        {   
            var count = 0;
            var type = node.GetType();
            for(int i = 0; i < asset.Count; i++)
            {
                var existingNode = asset.GetNodeInIteration(i);
                if (existingNode != null && existingNode.GetType() == type)
                    count++;
            }

            var primitiveName = string.Concat(type, count);
            return primitiveName.GetHashCode();
        }

        public static NodeDescriptor GetDefaultDescriptor<T>(T node, TreeAsset asset) where T : NodeBase
        {   
            var desc = Descriptors.Get(node);
            desc.Id = GetContextId(node, asset);
            desc.DisplayName = GetDefaultPrettyName(node);
            if(desc.DefaultWindowSize == Vector2.zero)
                desc.DefaultWindowSize = s_defaultSize;
            return desc;
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

        private static Dictionary<Type, NodeDescriptor> _descriptors = new Dictionary<Type, NodeDescriptor>()
        {
            { typeof(Root), new NodeDescriptor(_rootDesc, 1, 1, _defaultSize) },
            { typeof(Selector), new NodeDescriptor(_selectorDesc, -1, 2, _defaultSize) },
            { typeof(RandomSelector), new NodeDescriptor(_randSelectorDesc, -1, 2, _defaultSize) },
            { typeof(Sequence), new NodeDescriptor(_seqDesc, -1, 2, _defaultSize) },
            { typeof(RandomSequence), new NodeDescriptor(_randSeqDesc, -1, 2, _defaultSize) },
            { typeof(Inverter), new NodeDescriptor(_inverterDesc, 1, 1, _defaultSize) },
            { typeof(SeekNode), new NodeDescriptor(_seekDesc, 0, 0, _defaultSize) },
            { typeof(FindNode), new NodeDescriptor(_findDesc, 0, 0, _defaultSize) },
            { typeof(RepeatUntilFail), new NodeDescriptor(_findDesc, 1, 1, _defaultSize) },
            { typeof(RepeatUntilSucces), new NodeDescriptor(_findDesc, 1, 1, _defaultSize) },
        };

        public static NodeDescriptor Get<T>(T node) where T : NodeBase
        {
            if (_descriptors.TryGetValue(node.GetType(), out var descriptor))
                return descriptor;
            return default;
        }
    }

}
