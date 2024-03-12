using System;

namespace Aikom.AIEngine
{
    public abstract class LeafNode : NodeBase
    {
        public override bool IsValid()
        {
            return Parent != null;
        }
    }
}

