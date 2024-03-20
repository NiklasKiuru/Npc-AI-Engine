using System.Collections.Generic;
namespace Aikom.AIEngine.Editor
{
    public class Branch
    {
        public string Name { get; set; }
        public List<NodeBase> Nodes { get; private set; }
        
        public Branch(string name, List<NodeBase> nodes)
        {
            Name = name;
            Nodes = nodes;
        }
    }
}
