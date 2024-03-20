using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree/Tree")]
    public class TreeAsset : ScriptableObject
    {
        [ReadOnly][SerializeReference] Root _root;
        [ReadOnly][SerializeField] List<string> _localVariableNames = new List<string>();

        [ReadOnly][SerializeReference] private List<NodeBase> _nodes = new();

        public Root Root
        {
            get
            {
                if (_root == null)
                {
                    _root = new Root(-1);
//                    _root = CreateInstance<Root>();
//#if UNITY_EDITOR
//                    AssetDatabase.AddObjectToAsset(_root, this);
//                    EditorUtility.SetDirty(this);
//                    AssetDatabase.SaveAssets();
//#endif            
                }
                    
                return _root;
            }
        }

        public NodeBase this[int index]
        {
            get { return _nodes[index]; }
        } 

        public List<string> LocalVariables => _localVariableNames;
        public int Count => _nodes.Count;

        /// <summary>
        /// Removes a node from this asset
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNode(int id)
        {
            _nodes.Remove(_nodes.Find((n) => n.Id == id));
        }

        /// <summary>
        /// Adds a node to this asset
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="node"></param>
        public void AddNode(NodeBase node)
        {
            _nodes.Add(node);
        }

        public NodeBase GetNode(int id)
        {
            if (id == -1)
                return Root;
            for(int i = 0; i < _nodes.Count; i++)
            {
                if(_nodes[i].Id == id)
                    return _nodes[i];
            }
            return null;
        }

        /// <summary>
        /// Gets the depth of the tree
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            if(_nodes.Count == 0 || Root.ChildCount == 0) 
                return 0;

            var leaves = new List<LeafNode>();
            for (int i = 0; i < _nodes.Count; i++)
            {
                var wrapper = _nodes[i];
                if (wrapper is LeafNode leaf)
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
                    if(parent == null)
                    {
                        localDepth = 0;
                        break;
                    }
                    parent = parent.Parent as NodeBase;
                    if (parent is Root)
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
    }

}
