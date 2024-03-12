using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aikom.AIEngine.Editor
{
    public class BTNode : Node
    {
        private static readonly Color _validColor = new Color(0, 1, 0.2f, 0.50f);
        private static readonly Color _invalidColor = new Color(1, 0, 0, 0.50f);

        private NodeDescriptor _descriptor;
        private NodeBase _base;
        private TreeGraphView _view;
        private bool _hasValidConnections;

        /// <summary>
        /// Is this node valid by its connections?
        /// </summary>
        public bool IsValid => _hasValidConnections;

        /// <summary>
        /// Is this the root of the tree?
        /// </summary>
        public bool IsRoot => _base is Root;

        /// <summary>
        /// Visual parent node of this node. Always null on root
        /// </summary>
        public BTNode Parent 
        {
            get
            {
                if (IsRoot)
                    return null;
                return inputContainer[0] as BTNode;
            } 
        }

        /// <summary>
        /// Internal parent of this node. Always null on root
        /// </summary>
        public NodeBase BaseParent
        {
            get
            {
                if(IsRoot) 
                    return null;
                return Parent.Base;
            }
        }

        /// <summary>
        /// Internal node that this element represents
        /// </summary>
        public NodeBase Base => _base;

        /// <summary>
        /// Called on connection validation
        /// </summary>
        public event Action<BTNode> OnValidate;

        public BTNode(NodeBase node, string name, NodeDescriptor desc, TreeGraphView view) 
        {
            _descriptor = desc;
            _base = node;
            _view = view;

            title = name;
            // Parent
            if(node is not Root)
            {
                var port = CreatePort("", inputContainer, Direction.Input);
                port.OnConnect += SetParentOnConnect;
                port.OnDisconnect += VoidParent;
            }
                
            inputContainer.style.justifyContent = Justify.Center;
            inputContainer.style.alignItems = Align.Center;
            inputContainer.style.backgroundColor = outputContainer.style.backgroundColor;

            var childCount = _descriptor.MaxChildren < 0 ? _descriptor.MinChildren : _descriptor.MaxChildren == 0 ? 0 : 1;
            int currentChildren = 0;
            if (node is IParent parent)
                currentChildren = parent.ChildCount;
            childCount = Mathf.Max(childCount, currentChildren);
            if (childCount > 1)
            {
                var addButton = new Button(() => AddChildOption()) { text = "+", tooltip = "Add child" };
                var removeButton = new Button(() => RemoveChildOption()) { text = "-", tooltip = "Remove child" };
                titleButtonContainer.Add(addButton);
                titleButtonContainer.Add(removeButton);
            }
            else
            {
                titleContainer.style.justifyContent = Justify.Center;
                titleContainer.style.alignItems = Align.Center;
            }
            for(int i = 0; i < childCount; i++)
            {
                CreateChild();
            }
            outputContainer.style.flexDirection = FlexDirection.Row;
            outputContainer.style.justifyContent = Justify.SpaceAround;
            titleContainer.Q<Label>("title-label").style.marginLeft = 0;
            style.minWidth = _descriptor.DefaultWindowSize.x;

            RefreshAndDontExpand();
        }

        private void VoidParent(Port port) => SetParentForBase(null);
        private void SetParentOnConnect(Port port) => SetParentForBase((port.connections.First().output.node as BTNode).Base as IParent);
        private void SetParentForBase(IParent node) => _base.SetParent(node);

        /// <summary>
        /// Refreshes ports, connection status and styles without overriding current styles with expanded ones
        /// </summary>
        public void RefreshAndDontExpand()
        {
            RefreshExpandedState();
            RefreshPorts();
            UpdateValidityByConnections();
            
            var border = this.Q<VisualElement>("node-border");
            border.Insert(0, inputContainer);
            var collapser = border.Q<VisualElement>("collapse-button");
            collapser?.parent.Remove(collapser);
        }

        /// <summary>
        /// Updates the validity of this node visually
        /// </summary>
        public void UpdateValidityByConnections()
        {
            var inputStatus = UpdateValidity(inputContainer);
            var outputStatus = UpdateValidity(outputContainer);
            _hasValidConnections = inputStatus && outputStatus;
            OnValidate?.Invoke(this);
        }

        private bool UpdateValidity(VisualElement cont)
        {
            // Check port status'
            bool status = true;
            foreach (var element in cont.Children())
            {
                if (element is Port port)
                    status &= port.connected;
            }
            if (status)
                cont.style.backgroundColor = _validColor;
            else
                cont.style.backgroundColor = _invalidColor;
            return status;
        }

        private BTPort CreateChild()
        {
            var child = CreatePort(string.Concat(""), outputContainer, Direction.Output);
            child.AddManipulator(new EdgeConnector<Edge>(new BTEdgeConnectorDropoutListener(_view)));
            child.OnConnect += SetAsChild;
            child.OnDisconnect += VoidChild;
            return child;
        }

        private void SetAsChild(Port port) => SetChild(port, (port.connections.First().input.node as BTNode).Base);
        private void VoidChild(Port port) => SetChild(port, null);
        private void SetChild(Port port, NodeBase node)
        {
            var index = GetPortIndex(port, outputContainer);
            if (index < 0)
                return;
            (_base as IParent).SetChild(index, node);
            Debug.Log("Connected parent: " + _base.GetType().ToString() + " to: " + node?.GetType().ToString());
        }

        private int GetPortIndex(Port port, VisualElement cont)
        {
            var index = 0;
            foreach(var element in cont.Children())
            {
                if(element.Equals(port))
                    return index;
                index++;
            }
            return -1;
        }


        private BTPort CreatePort(string name, VisualElement container, Direction dir)
        {
            BTPort port = BTPort.Create<Edge>(Orientation.Vertical, dir, Port.Capacity.Single, typeof(NodeStatus));
            port.portName = name;
            var label = port.Q<Label>("type");
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            container.Add(port);
            return port;
        }

        private void AddChildOption() 
        { 
            if(outputContainer.childCount < _descriptor.MaxChildren || _descriptor.MaxChildren < 0)
            {
                CreateChild();
                RefreshAndDontExpand();
            }
        }

        private void RemoveChildOption()
        {
            if(outputContainer.childCount > _descriptor.MinChildren)
            {
                var port = outputContainer[childCount] as Port;
                if(port.connected)
                {   
                    var btOut = port.connections.First().input.node as BTNode;
                    (btOut.inputContainer[0] as BTPort).DisconnectAll();
                    _view.DeleteElements(port.connections);

                }
                outputContainer.RemoveAt(outputContainer.childCount - 1);
                RefreshAndDontExpand();
            }
        }

        /// <summary>
        /// Gets and updates the descriptor
        /// </summary>
        /// <returns></returns>
        public NodeDescriptor GetDescriptor()
        {
            _descriptor.SetPosition(GetPosition());
            return _descriptor;
        }

        /// <summary>
        /// Called on selection
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            _view.ShowNodeProperties(this);
        }

        /// <summary>
        /// Called on unselection
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();
            _view.ShowNodeProperties(null);
        }

        /// <summary>
        /// Sets the title and internal name
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _descriptor.SetName(name);
            title = name;
        }

        /// <summary>
        /// Unlinks the OnConnect and OnDisconnect calls
        /// </summary>
        internal void UnLink()
        {
            foreach(var port in outputContainer.Children())
            {
                if(port is BTPort btPort)
                {
                    btPort.OnConnect -= SetAsChild;
                    btPort.OnDisconnect -= VoidChild;
                }
            }
            if (!IsRoot)
            {
                var port = inputContainer.Children().First() as BTPort;
                port.OnConnect -= SetParentOnConnect;
                port.OnDisconnect -= VoidParent;
            }
                
        }
    }

}
