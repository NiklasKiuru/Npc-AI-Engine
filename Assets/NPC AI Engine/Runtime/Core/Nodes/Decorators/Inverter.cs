namespace Aikom.AIEngine
{
    public class Inverter : DecoratorNode
    {


        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            var substatus = Child.Process();
            return Invert(substatus);
        }

        public override void OnBackPropagate(NodeStatus status)
        {
            Parent.OnBackPropagate(Invert(status));
        }

        private NodeStatus Invert(NodeStatus status)
        {
            if (status == NodeStatus.Succes)
                return NodeStatus.Failure;
            else if (status == NodeStatus.Failure)
                return NodeStatus.Succes;
            return status;
        }

        public override bool IsValid()
        {
            return Child != null;
        }

        protected override void OnBuild()
        {
        }
    }

}
