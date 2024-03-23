using System.Collections.Generic;
namespace Aikom.AIEngine.Editor
{   
    /// <summary>
    /// Data container that represents a template to store and duplicate parts of a tree structure
    /// </summary>
    public class Branch
    {   
        /// <summary>
        /// Name identifier for this branch
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Template nodes used to create this branch
        /// </summary>
        public List<NodeBase> Nodes { get; private set; }
        
        public Branch(string name, List<NodeBase> nodes)
        {
            Name = name;
            Nodes = nodes;
        }
    }
}
