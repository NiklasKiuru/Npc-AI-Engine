using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class EqualsNode : LeafNode
    {
        [ExposedVariable("Object 1 location")]
        private CacheSpace _space1;

        [ExposedVariable("Object 1")]
        private string _cacheRead1;

        [ExposedVariable("Object 1 location")]
        private CacheSpace _space2;

        [ExposedVariable("Object 2")]
        private string _cacheRead2;

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            var val1 = Read(_space1, _cacheRead1);
            var val2 = Read(_space2, _cacheRead2);

            if (val1 != val2)
                return NodeStatus.Failure;
            return NodeStatus.Succes;
        }

        private object Read(CacheSpace space, string name)
        {
            if(space == CacheSpace.Local)
                return Context.GetLocalVariable(name);
            return Context.GetGlobalVariable(name);
        }
    }

}
