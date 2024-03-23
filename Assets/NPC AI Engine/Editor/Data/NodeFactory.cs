using System;
using UnityEditor;
using UnityEngine;

namespace Aikom.AIEngine.Editor
{   
    // **NOTES** Should propably just be removed

    /// <summary>
    /// Factory for nodes
    /// </summary>
    public static class NodeFactory
    {   
        /// <summary>
        /// Creates a new node
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NodeBase CreateNew(Type type)
        {
            var id = GUID.Generate().ToString().GetHashCode();
            if (type == typeof(Root))
                id = -1;
            var baseNode = Activator.CreateInstance(type, id) as NodeBase;
            NodeDescriptor descriptor;
            Vector2Int conf = NodeDescriptor.DefaultChildConfiguration(baseNode);
            descriptor = new NodeDescriptor(conf.x, conf.y);
            descriptor.DisplayName = NodeDescriptor.GetDefaultPrettyName(baseNode);
            baseNode.Descriptor = descriptor;
            return baseNode;
        }
    }
}
