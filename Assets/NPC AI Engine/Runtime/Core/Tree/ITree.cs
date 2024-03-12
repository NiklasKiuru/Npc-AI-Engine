using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public interface ITree : IContext
    {
        public float TickDelta { get; }
        public void CacheNode(NodeBase node);
        public Root GetRoot();
    }
}
