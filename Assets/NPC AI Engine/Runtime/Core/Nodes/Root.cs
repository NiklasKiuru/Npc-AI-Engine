namespace Aikom.AIEngine
{
    public sealed class Root : NodeBase, IParent
    {
        private NodeBase _child;

        public Root(int id) : base(id)
        {
        }

        private Root(int id, Position pos) : base(id, pos)
        {
        }

        public int ChildCount => _child == null ? 0 : 1;
        public bool IsCached { get; set; }

        public bool AddChild(NodeBase node)
        {   
            if(_child == null)
            {
                _child = node;
                return true;
            }
            return false;
        }

        public NodeBase GetChild(int index) => _child;

        public void OnBackPropagate(NodeStatus status, INode sender)
        {
            // Notify context and abort tree
            if(status != NodeStatus.Cached)
            {
                Context.CacheNode(this);
            }
        }

        protected override NodeStatus Tick()
        {   
            var subStatus = _child.Process();
            if(subStatus != NodeStatus.Cached)
            {
                Context.CacheNode(this);
            }

            return subStatus;
        }

        public void RemoveChild(int index) => _child = null;

        public void SetChild(int index, NodeBase node) => _child = node;

        protected override void OnInit()
        {
        }

        public override bool IsValid(out string message)
        {
            message = "Root does not contain a valid child to process";
            return _child != null;
        }

        protected override void OnBuild()
        {
        }

        public override INode Clone()
        {
            return new Root(Id, Position);
        }
    }

}
