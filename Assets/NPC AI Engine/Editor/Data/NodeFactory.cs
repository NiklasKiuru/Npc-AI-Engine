using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Aikom.AIEngine.Editor
{
    public static class NodeFactory
    {
        public static NodeBase CreateNew(Type type)
        {
            var id = GUID.Generate().ToString().GetHashCode();
            if (type == typeof(Root))
                id = -1;
            var baseNode = Activator.CreateInstance(type, id) as NodeBase;
            NodeDescriptor descriptor;
            Vector2Int conf = NodeDescriptor.DefaultChildConfiguration(baseNode);
            var editorAttr = type.GetCustomAttribute<EditorNodeAttribute>();
            if (editorAttr != null)
                descriptor = new NodeDescriptor(editorAttr.Description, conf.x, conf.y);
            else
                descriptor = new NodeDescriptor("", conf.x, conf.y);
            descriptor.DisplayName = NodeDescriptor.GetDefaultPrettyName(baseNode);
            baseNode.Descriptor = descriptor;
            return baseNode;
        }
    }
}
