using UnityEngine;

namespace Aikom.AIEngine
{
    [EditorNode("Reads a value from one cache space to another")]
    public class ReadValue : LeafNode 
    {   
        [Tooltip("Read from"), SerializeField]
        private string _cacheRead;

        [Tooltip("Write to"), SerializeField]
        private string _localCacheWrite;

        [Tooltip("Cache space")]
        private CacheSpace _space;

        public ReadValue(int id) : base(id)
        {
        }

        protected ReadValue(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            var newNode = new ReadValue(Id, Position);
            newNode._cacheRead = _cacheRead;
            newNode._localCacheWrite = _localCacheWrite;
            newNode._space = _space;
            return newNode;
        }

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            switch (_space)
            {
                case CacheSpace.Global:
                    Context.SetLocalVariable(_localCacheWrite, Context.GetGlobalVariable(_cacheRead));
                    break;
                case CacheSpace.Local:
                    Context.SetLocalVariable(_localCacheWrite, Context.GetLocalVariable(_cacheRead));
                    break;
            }
            return NodeStatus.Succes;
        }

    }

    public enum CacheSpace
    {
        Local,
        Global
    }

}
