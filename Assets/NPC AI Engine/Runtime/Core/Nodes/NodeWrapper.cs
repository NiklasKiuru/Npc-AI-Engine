using System;
using UnityEngine;
namespace Aikom.AIEngine
{
    [Serializable]
    public class NodeWrapper
    {
        [SerializeField] private NodeBase _node;
        [SerializeField] private NodeDescriptor _descriptor;

        public NodeBase Node { get => _node; }
        public NodeDescriptor Descriptor { get => _descriptor; set => _descriptor = value; }
        public int Id => Descriptor.Id;

        public NodeWrapper(NodeBase node, NodeDescriptor descriptor)
        {
            _node = node;
            _descriptor = descriptor;
        }
    }
}
