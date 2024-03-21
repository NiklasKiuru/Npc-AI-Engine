using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System;
using UnityEditor.UIElements;
using Codice.Client.Common.FsNodeReaders;

namespace Aikom.AIEngine.Editor
{
    public class TreeGraphView : GraphView
    {
        private NodeSearchWindow _searchWindow;
        private TreeEditor _editorWindow;
        private CachedEdgeEvent _delayedConnection;
        private TreeAsset _treeAsset;
        //private BTNode _selectedNode;

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
                        _treeAsset.RemoveNode(visNode.Base.Id);
                    }
                }

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(_treeAsset);
                DeleteSelection();
            };
        }

        public void OnRuntimeTick(INode node, NodeStatus status)
        {
            foreach(BTNode nodeElement in nodes)
            {
                var border = nodeElement.Q<VisualElement>("selection-border");
                if(nodeElement.Base.Id == node.Id)
                {
                    border.style.borderBottomWidth = 3;
                    var color = new Color();
                    switch (status)
                    {
                        case NodeStatus.Succes:
                            color = Color.green; 
                            break;
                        case NodeStatus.Failure:
                            color = Color.red;
                            break;
                        case NodeStatus.Cached:
                            color = Color.blue; 
                            break;
                        case NodeStatus.Running:
                            color = Color.gray;
                            break;
                    }
                    var targetColor = new Color(color.r, color.g, color.b, 0);
                    nodeElement.experimental.animation.Start(
                        color, targetColor, 250,
                        (v, c) => border.style.borderBottomColor = c);
                }
            }
        }

        public void OnRuntimeBackPropagate(INode sender, IParent reciever)
        {
            foreach(BTNode nodeElement in nodes)
            {
                var border = nodeElement.Q<VisualElement>("selection-border");
                if(nodeElement.Base.Id == sender.Id)
                {
                    border.style.borderTopWidth = 3;
                    var c = Color.yellow;
                    var target = new Color(c.r, c.g, c.b, 0);
                    nodeElement.experimental.animation.Start(
                        c, target, 250,
                        (v, c) => border.style.borderTopColor = c);
                }
            }
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
                // Update descriptors and position
                var desc = node.UpdateDescriptor();
                if (node.Base is not IParent)
                    node.Base.UpdatePositionData(new Position() { inputId = node.Base.Parent == null ? 0 : node.Base.Parent.Id, outputIds = new() });               

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
                    while(emptyChildren > 0 && parent.ChildCount - emptyChildren >= desc.MinChildren)
                    {   
                        var pos = emptyIndecies[index];
                        node.outputContainer.RemoveAt(pos);
                        parent.RemoveChild(pos);
                        emptyChildren--;
                    }
                    node.Base.UpdatePositionData(parent.GetPosition());
                    node.UpdateValidityByConnections();
                    EditorUtility.SetDirty(_treeAsset);
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
                var node = _treeAsset[i];
                var desc = node.Descriptor;
                CreateDisplayNode(node, desc.Position, desc.DisplayName);
            }

            // Connections
            foreach(BTNode visNode in nodes)
            {
                if(visNode.Base is IParent parent)
                {
                    var pos = visNode.Base.Position;
                    for(int i = 0; i < pos.outputIds.Count; i++)
                    {
                        if (pos.outputIds[i] != 0)
                        {
                            var child = _treeAsset.GetNode(pos.outputIds[i]);
                            if (child == null)
                                throw new Exception("Something went wrong with previous serialization cycle");
                            var port = visNode.outputContainer[i] as BTPort;
                            var visChild = nodes.First((n) => { return (n as BTNode).Base.Id == child.Id; });
                            var edge = port.ConnectTo(visChild.inputContainer[0] as BTPort);
                            AddElement(edge);
                        }
                    }
                }
            }
        }

        public void ShowNodeProperties(BTNode node)
        {   
            //_selectedNode = node;
            //NodePropertyBoard.Clear();
            if (node == null)
            {
                _editorWindow.OnNodeSelected(null);
                return;
            }
            else   
            {
                _editorWindow.OnNodeSelected(node.Base);
                return;
            }
                

            //var baseNode = node.Base;
            //var desc = baseNode.Descriptor;
            //var nameField = new TextField("Name") { value = desc.DisplayName };
            //nameField.RegisterValueChangedCallback((v) => _selectedNode.SetName(v.newValue));
            //NodePropertyBoard.title = NodeDescriptor.GetDefaultPrettyName(baseNode) + " || Debug Id: " + baseNode.Id;
            //NodePropertyBoard.contentContainer.Add(nameField);
            //NodePropertyBoard.subTitle = desc.Description;
            //var fields = FieldSerializationUtility.GetPropertyElements(baseNode, _treeAsset);
            //foreach ( var field in fields)
            //{
            //    if (field != null)
            //        NodePropertyBoard.contentContainer.Add(field);
            //}

            //if(baseNode is Root)
            //{
            //    var childCountElement = new TextField("Total child count") { value = _treeAsset.Count.ToString() };
            //    childCountElement.SetEnabled(false);
            //    var depthElement = new IntegerField("Total valid depth") { value = _treeAsset.GetDepth() };
            //    depthElement.SetEnabled(false);
            //    NodePropertyBoard.contentContainer.Add(childCountElement);
            //    NodePropertyBoard.contentContainer.Add(depthElement);
            //}

            //if(customEditor != null)
            //    UnityEngine.Object.DestroyImmediate(customEditor);
            //_editorWindow.CustomEditor = UnityEditor.Editor.CreateEditor(_treeAsset);
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
        public void CreateNewDisplayNode(Type type, Vector2 pos)
        {   
            if(_treeAsset == null) 
                return;
            var node = NodeFactory.CreateNew(type);
            var desc = node.Descriptor;
            var uiNode = CreateDisplayNode(node, new Rect(pos, desc.DefaultWindowSize), desc.DisplayName);
            
            if(_delayedConnection != null)
            {
                if (uiNode.inputContainer[0] is Port port)
                {
                    var edge = port.ConnectTo(_delayedConnection.Origin);
                    AddElement(edge);
                }
            }
            _delayedConnection = null;

            _treeAsset.AddNode(node);
            EditorUtility.SetDirty(_treeAsset);
            AssetDatabase.SaveAssets();
        }

        private BTNode CreateDisplayNode(NodeBase node, Rect pos, string name = "")
        {
            var uiNode = new BTNode(node, name, this);
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
            var desc = root.Descriptor;
            if (root.Descriptor.Position == Rect.zero)
                root.SetWindowPosition(new Rect(new Vector2(_editorWindow.position.xMax / 2, 
                    _editorWindow.position.yMax / 2), desc.DefaultWindowSize));
            var node = CreateDisplayNode(root, root.Descriptor.Position, "Root");
            node.capabilities &= ~Capabilities.Deletable;
            return node;
        }
    }
}

