namespace Aikom.AIEngine
{
    public class RepeatUntilSucces : DecoratorNode
    {
        public override void OnBackPropagate(NodeStatus status)
        {
            if (status != NodeStatus.Succes)
                Tick();
            else
                Parent.OnBackPropagate(status);
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
