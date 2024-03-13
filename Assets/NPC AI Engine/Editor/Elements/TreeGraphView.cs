using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UIElements;

namespace Aikom.AIEngine.Editor
{
    public class TreeGraphView : GraphView
    {
        private NodeSearchWindow _searchWindow;
        private TreeEditor _editorWindow;
        private CachedEdgeEvent _delayedConnection;
        private TreeAsset _treeAsset;
        private BTNode _selectedNode;

        public Blackboard NodePropertyBoard { get; set; }

        private class CachedEdgeEvent
        {
            public Port Origin;
            public Port Target;

            public CachedEdgeEvent(Port origin)
            {
                Origin = origin;
            }
        }

        public TreeGraphView(TreeEditor window) 
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GridStyle"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            _editorWindow = window;
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(window, this);
            nodeCreationRequest = context =>
            {   
                if(_treeAsset != null)
                    SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
            };
            deleteSelection = (message, ask) =>
            {
                foreach(var node in selection)
                {
                    if(node is BTNode visNode)
                    {
                        _treeAsset.RemoveNode(visNode.GetDescriptor().Id);
                    }
                }

                DeleteSelection();
            };
        }

        /// <summary>
        /// Populates the tree visually
        /// </summary>
        /// <param name="asset"></param>
        public void Populate(TreeAsset asset)
        {   
            // No new tree
            if (asset == null)
            {   
                _treeAsset = null;
                ClearGraph();
                return;
            }
            _treeAsset = asset;
            BuildVisualTree();
        }

        /// <summary>
        /// Builds and validates the tree internally. Must be called before saving the tree asset
        /// </summary>
        public void TrimAndBuildAssetTree()
        {
            if (_treeAsset == null)
                return;
            foreach(BTNode node in nodes)
            {
                // Update descriptors
                var desc = node.GetDescriptor();
                _treeAsset.UpdateDescriptor(desc);

                // Remove unused child slots from composites
                if (node.Base is IParent parent)
                {
                    var emptyIndecies = new List<int>(); 
                    int childCount = parent.ChildCount;
                    int emptyChildren = 0;
                    for (int i = 0; i < childCount; i++)
                    {
                        if(parent.GetChild(i) == null)
                        {
                            emptyIndecies.Add(i);
                            emptyChildren++;
                        }
                            
                    }
                    var index = 0;
                    while(emptyChildren > 0 && parent.ChildCount - emptyChildren > desc.MinChildren)
                    {
                        parent.RemoveChild(emptyIndecies[index]);
                        emptyChildren--;
                    }
                }

            }
        }

        private void ClearGraph()
        {   
            foreach(BTNode node in nodes)
                node.UnLink();
            DeleteElements(nodes);
            DeleteElements(edges);
        }

        private void BuildVisualTree()
        {
            // Clear previous nodes
            ClearGraph();

            // Create new ones
            CreateRoot(_treeAsset.Root);
            for(int i = 0; i < _treeAsset.Count; i++)
            {   
                var wrapper = _treeAsset[i];
                var desc = wrapper.Descriptor;
                CreateNode(wrapper.Node, desc.Position, desc, desc.DisplayName);
            }

            // Connections
            foreach(BTNode visNode in nodes)
            {
                if(visNode.Base is IParent parent && parent.ChildCount > 0)
                {   
                    for(int i = 0; i < parent.ChildCount; i++)
                    {
                        var child = parent.GetChild(i);
                        var port = visNode.outputContainer[i] as BTPort;
                        var visChild = nodes.First((n) => { return (n as BTNode).Base == child; });
                        var edge = port.ConnectTo(visChild.inputContainer[0] as BTPort);
                        AddElement(edge);
                    }
                }
            }

        }

        public Root GetRoot()
        {
            if(_treeAsset != null)
                return _treeAsset.Root;
            return null;
        }

        public void ShowNodeProperties(BTNode node)
        {   
            _selectedNode = node;
            NodePropertyBoard.Clear();
            if (node == null)
                return;

            var baseNode = node.Base;
            var nameField = new TextField("Name") { value = node.GetDescriptor().DisplayName };
            nameField.RegisterValueChangedCallback((v) => _selectedNode.SetName(v.newValue));
            NodePropertyBoard.title = NodeDescriptor.GetDefaultPrettyName(baseNode);
            NodePropertyBoard.contentContainer.Add(nameField);
            NodePropertyBoard.subTitle = node.GetDescriptor().Description;
            var fields = FieldSerializationUtility.GetPropertyElements(baseNode, _treeAsset);
            foreach ( var field in fields)
            {
                if (field != null)
                    NodePropertyBoard.contentContainer.Add(field);
            }

            if(baseNode is Root)
            {
                var childCountElement = new TextField("Total child count") { value = _treeAsset.Count.ToString() };
                childCountElement.SetEnabled(false);
                var depthElement = new IntegerField("Total valid depth") { value = _treeAsset.GetDepth() };
                depthElement.SetEnabled(false);
                NodePropertyBoard.contentContainer.Add(childCountElement);
                NodePropertyBoard.contentContainer.Add(depthElement);
            }
        }

        /// <summary>
        /// Caches the port if the edge is dropped outside a valid connector for creator window
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="pos"></param>
        public void OnDropCreate(Edge edge, Vector2 pos)
        {
            if(_delayedConnection == null && edge.output.connections.Count() < 1)
                _delayedConnection = new CachedEdgeEvent(edge.output);
            nodeCreationRequest.Invoke(new NodeCreationContext() { screenMousePosition = pos });
        }

        /// <summary>
        /// Creates a new default display node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pos"></param>
        public void CreateNewDisplayNode(NodeBase node, Vector2 pos)
        {   
            if(_treeAsset == null) 
                return;
            var desc = NodeDescriptor.GetDefaultDescriptor(node, _treeAsset);
            var name = NodeDescriptor.GetDefaultPrettyName(node);
            var uiNode = CreateNode(node, new Rect(pos, desc.DefaultWindowSize), desc, name);
            
            if(_delayedConnection != null)
            {
                if (uiNode.inputContainer[0] is Port port)
                {
                    var edge = port.ConnectTo(_delayedConnection.Origin);
                    AddElement(edge);
                    //uiNode.UpdateValidityByConnections();
                }
            }
            _delayedConnection = null;

            _treeAsset.AddNode(desc, node);
            EditorUtility.SetDirty(_treeAsset);
        }

        private BTNode CreateNode(NodeBase node, Rect pos, NodeDescriptor desc, string name = "")
        {
            var uiNode = new BTNode(node, name, desc, this);
            uiNode.SetPosition(pos);
            AddElement(uiNode);
            return uiNode;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != portView.direction)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private BTNode CreateRoot(Root root)
        {
            var desc = NodeDescriptor.GetDefaultDescriptor(root, _treeAsset);
            var node = CreateNode(root, new Rect(new Vector2(_editorWindow.position.xMax /2, _editorWindow.position.yMax / 2), desc.DefaultWindowSize), desc, "Root");
            node.capabilities &= ~Capabilities.Deletable;
            node.capabilities &= ~Capabilities.Movable;
            return node;
        }
    }
}

