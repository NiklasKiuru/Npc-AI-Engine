namespace Aikom.AIEngine
{
    public class RepeatUntilFail : DecoratorNode
    {
        public RepeatUntilFail(int id) : base(id)
        {
        }

        protected RepeatUntilFail(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            return new RepeatUntilFail(Id, Position);
        }

        public override void OnBackPropagate(NodeStatus status, INode sender)
        {
            if (status != NodeStatus.Failure)
                Tick();
            else
                this.StartBackPropagation(status, Parent);
        }

        protected override void OnBuild()
        {
        }

        protected override NodeStatus Tick()
        {   
            var subState = Child.Process();
            if(subState != NodeStatus.Failure)
            {
                Context.CacheNode(this);
                return NodeStatus.Cached;
            }
            else if (IsCached)
                this.StartBackPropagation(subState, Parent);

            return NodeStatus.Failure;
        }
    }

}
