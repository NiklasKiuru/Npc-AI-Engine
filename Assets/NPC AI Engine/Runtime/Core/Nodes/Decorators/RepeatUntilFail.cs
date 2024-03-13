namespace Aikom.AIEngine
{
    public class RepeatUntilFail : DecoratorNode
    {   
        public override void OnBackPropagate(NodeStatus status)
        {
            if(status != NodeStatus.Failure)
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
