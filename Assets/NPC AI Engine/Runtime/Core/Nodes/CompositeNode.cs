using System.Collections.Generic;

namespace Aikom.AIEngine
{
    public abstract class CompositeNode : NodeBase, IParent
    {   
        private List<NodeBase> _children = new(2);
        private bool _isCached;
        private int _processIndex;
        private NodeStatus _breakStatus;

        /// <summary>
        /// Composite children. Either Leafs, Decorators or other Composites
        /// </summary>
        protected List<NodeBase> Children { get { return _children; } }

        /// <summary>
        /// Determines wether this node is cached in the engine in its current process cycle
        /// </summary>
        protected bool IsCached { get { return _isCached; } set { _isCached = value; } }

        /// <summary>
        /// Cached process index
        /// </summary>
        protected int ProcessIndex { get { return _processIndex; } set { _processIndex = value; } }

        /// <summary>
        /// Status that breaks the loop automatically
        /// </summary>
        protected NodeStatus BreakStatus { get { return _breakStatus; } set { _breakStatus = value; } }

        public int ChildCount 
        {
            get
            {
                if (_children == null)
                    return 0;
                return _children.Count;
            }
        }

        public bool AddChild(NodeBase node)
        {
            _children.Add(node);
            return true;
        }

        public NodeBase GetChild(int index) => _children[index];
        public void RemoveChild(int index) => _children.RemoveAt(index);
        public void SetChild(int index, NodeBase node) 
        { 
            if(index >= ChildCount)
            {
                var temp = new List<NodeBase>();
                for(int i = 0; i <= index; i++)
                {
                    if(i < ChildCount)
                        temp.Add(_children[i]);
                    else if(i == index)
                        temp.Add(node);
                    else
                        temp.Add(null);
                }
                _children = temp;
            }
            else
                _children[index] = node;
        }
        public virtual void OnBackPropagate(NodeStatus status)
        {
            ProcessIndex++;
            if (status == _breakStatus || ProcessIndex >= ChildCount)
                StartBackPropagation(status);
            else
                Tick();
        }
        public override bool IsValid()
        {
            return ChildCount >= 2;
        }

        protected override NodeStatus Tick()
        {
            var finalStatus = NodeStatus.Undefined;
            for (int i = ProcessIndex; i < Children.Count; i++)
            {
                var subStatus = ProcessChild(i);
                if (subStatus == NodeStatus.Cached)
                    return NodeStatus.Cached;
                if (subStatus == NodeStatus.Running)
                {
                    Context.CacheNode(this);
                    IsCached = true;
                    return NodeStatus.Cached;
                }
                else if (subStatus == _breakStatus)
                {
                    if (IsCached)
                    {
                        StartBackPropagation(subStatus);
                        break;
                    }
                    else
                        return subStatus;
                }
                finalStatus = subStatus;
            }
            if (IsCached)
                StartBackPropagation(finalStatus);
            return finalStatus;
        }

        protected virtual NodeStatus ProcessChild(int listIndex)
        {
            return Children[listIndex].Process();
        }

        protected void StartBackPropagation(NodeStatus status)
        {
            IsCached = false;
            Parent.OnBackPropagate(status);
        }

        protected override void OnInit()
        {
            _processIndex = 0;
        }
    }
}

