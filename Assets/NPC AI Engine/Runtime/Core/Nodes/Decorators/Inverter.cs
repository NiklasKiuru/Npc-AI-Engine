namespace Aikom.AIEngine
{
    public class Inverter : DecoratorNode
    {
        public Inverter(int id) : base(id) { }
        protected Inverter(int id, Position pos) : base(id, pos) { }

        protected override void OnInit()
        {
        }

        protected override NodeStatus Tick()
        {
            var substatus = Child.Process();
            return Invert(substatus);
        }

        public override void OnBackPropagate(NodeStatus status, INode sender)
        {
            this.StartBackPropagation(Invert(status), Parent);
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

        public override INode Clone()
        {
            return new Inverter(Id, Position);
        }
    }

}
