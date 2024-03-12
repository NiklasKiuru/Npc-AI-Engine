namespace Aikom.AIEngine
{
    public class ReadValue : LeafNode 
    {   
        [ExposedVariable("Read from")]
        private string _cacheRead;

        [ExposedVariable("Write to")]
        private string _localCacheWrite;

        [ExposedVariable("Cache space")]
        private CacheSpace _space;

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
