using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree/Tree")]
    public class TreeAsset : ScriptableObject
    {
        [HideInInspector][SerializeField] Root _root;
        [ReadOnly][SerializeField] List<string> _LocalVariableNames = new List<string>();

        private List<NodeWrapper> _nodes = new();
        private Dictionary<int, NodeWrapper> _lookUp = new();

        public Root Root
        {
            get
            {
                if (_root == null)
                    _root = CreateInstance<Root>();
                return _root;
            }
        }

        public NodeWrapper this[int index]
        {
            get { return _nodes[index]; }
        } 

        public List<string> LocalVariables => _LocalVariableNames;
        public int Count => _nodes.Count;

        /// <summary>
        /// Removes a node from this asset
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNode(int id)
        {
            CheckSerialization();
            _lookUp.Remove(id);
            var index = 0;
            for(int i =  0; i < _nodes.Count; i++)
            {
                if (_nodes[i].Id == id)
                {
                    index = i; 
                    break;
                }
            }
            _nodes.RemoveAt(index);
        }

        /// <summary>
        /// Adds a node to this asset
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="node"></param>
        public void AddNode(NodeDescriptor desc, NodeBase node)
        {
            CheckSerialization();
            var wrapper = new NodeWrapper(node, desc);
            _nodes.Add(wrapper);
            _lookUp.Add(wrapper.Id, wrapper);
        }

        public NodeBase GetNode(int id)
        {
            CheckSerialization();
            if (_lookUp.TryGetValue(id, out var val))
                return val.Node;
            return null;
        }

        public NodeBase GetNodeInIteration(int index)
        {
            return _nodes[index].Node;
        }

        public NodeDescriptor GetDescriptor(int id)
        {
            CheckSerialization();
            if(_lookUp.TryGetValue(id, out var val))
                return val.Descriptor;
            return default;
        }

        public void UpdateDescriptor(NodeDescriptor desc)
        {
            CheckSerialization();
            if (_lookUp.TryGetValue(desc.Id, out var wrapper))
            {
                wrapper.Descriptor = desc;
            } 
        }

        /// <summary>
        /// Gets the depth of the tree
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            if(_nodes.Count == 0) 
                return 0;

            var leaves = new List<LeafNode>();
            for (int i = 0; i < _nodes.Count; i++)
            {
                var wrapper = _nodes[i];
                if (wrapper.Node is LeafNode leaf)
                    leaves.Add(leaf);
            }

            int maxDepth = 0;
            
            for (int i = 0; i < leaves.Count; i++)
            {
                NodeBase parent = leaves[i].Parent as NodeBase;
                int localDepth = 1;
                while(true)
                {
                    localDepth++;
                    parent = parent.Parent as NodeBase;
                    if (parent is Root || parent == null)
                        break;
                }
                if(localDepth > maxDepth)
                        maxDepth = localDepth;
            }
            return maxDepth;
        }

        /// <summary>
        /// Gets the count of all children that are attached to root
        /// </summary>
        /// <returns></returns>
        public int GetChildCount()
        {
            if(_nodes.Count == 0)
                return 0;

            return -1;
        }

        // Checks if the dictionary has been lost on serialization
        private void CheckSerialization()
        {
            if(_nodes.Count != _lookUp.Count)
            {   
                _lookUp.Clear();
                for(int i = 0; i < _nodes.Count; i++)
                {
                    var node = _nodes[i];
                    _lookUp.TryAdd(node.Id, node);
                }
            }
        }
    }

}
