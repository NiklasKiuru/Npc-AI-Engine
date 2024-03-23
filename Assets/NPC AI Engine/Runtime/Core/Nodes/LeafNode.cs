using System;

namespace Aikom.AIEngine
{
    [Serializable]
    public abstract class LeafNode : NodeBase
    {
        protected LeafNode(int id) : base(id)
        {
        }

        protected LeafNode(int id, Position pos) : base(id, pos)
        {
        }

        public override bool IsValid(out string message)
        {   
            message = "No parent assigned";
            return Parent != null;
        }
    }
}

