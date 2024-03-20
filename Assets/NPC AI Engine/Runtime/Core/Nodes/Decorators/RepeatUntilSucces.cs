namespace Aikom.AIEngine
{
    public class RepeatUntilSucces : DecoratorNode
    {
        public RepeatUntilSucces(int id) : base(id)
        {
        }

        protected RepeatUntilSucces(int id, Position pos) : base(id, pos) { }

        public override INode Clone()
        {
            return new RepeatUntilSucces(Id, Position);
        }

        public override void OnBackPropagate(NodeStatus status, INode sender)
        {
            if (status != NodeStatus.Succes)
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
            if (subState != NodeStatus.Succes)
            {
                Context.CacheNode(this);
                return NodeStatus.Cached;
            }
            else if (IsCached)
                this.StartBackPropagation(subState, Parent);

            return NodeStatus.Succes;
        }
    }

}
