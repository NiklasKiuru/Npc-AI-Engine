using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aikom.AIEngine.Editor
{   
    /// <summary>
    /// Main editor window that holds the graph for node editing
    /// </summary>
    [Serializable]
    public class TreeEditor : EditorWindow
    {
        [SerializeField] private BranchContainer _branchCont;
        [SerializeField] private TreeAsset _asset;

        private TreeGraphView _graph;
        private Blackboard _localVariables;
        private BehaviourTree _cachedRuntimeTree;
        private IMGUIContainer _inspector;
        private VisualElement _twoPaneSplitView;
        private SerializedObject _assetSo;

        internal TreeAssetEditor CustomEditor { get; set; }

        [MenuItem("Window/Behaviour Tree")]
        public static void Init()
        {
            var window = GetWindow<TreeEditor>();
            window.titleContent = new GUIContent("Behaviour Tree");
            window.Show();
        }

        #region Engine callbacks
        private void CreateGUI()
        {
            _twoPaneSplitView = new TwoPaneSplitView(0, position.xMax * 0.2f, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(_twoPaneSplitView);
            _inspector = new IMGUIContainer();
            _twoPaneSplitView.Add(_inspector);

            CreateGraph();
            LoadTemplates();
            CreateToolbar();

            EditorApplication.quitting += SaveAsset;

            if (_asset != null)
            {
                LoadAsset(_asset);
            }  
        }

        private void OnDestroy()
        {
            EditorApplication.quitting -= SaveAsset;
            DestroyImmediate(CustomEditor);
        }

        private void OnSelectionChange()
        {
            if (_cachedRuntimeTree != null)
            {
                _cachedRuntimeTree.OnTickCallback -= _graph.OnRuntimeTick;
                _cachedRuntimeTree.OnBackpropagateCallback -= _graph.OnRuntimeBackPropagate;
            }
            if (Application.isPlaying)
            {
                if(Selection.activeObject != null && Selection.activeGameObject.TryGetComponent<NPCBrain>(out var brain) && brain.IsActive)
                {   
                    // Get field values
                    var treeInfo = typeof(NPCBrain).GetField("_tree", BindingFlags.NonPublic | BindingFlags.Instance);
                    var assetInfo = typeof(NPCBrain).GetField("_asset", BindingFlags.NonPublic | BindingFlags.Instance);
                    _cachedRuntimeTree = (BehaviourTree)treeInfo.GetValue(brain);
                    var asset = (TreeAsset)assetInfo.GetValue(brain);

                    LoadAsset(asset);
                    _cachedRuntimeTree.OnTickCallback += _graph.OnRuntimeTick;
                    _cachedRuntimeTree.OnBackpropagateCallback += _graph.OnRuntimeBackPropagate;
                }
                else
                {
                    _cachedRuntimeTree = null;
                }
            }
        }

        private void OnDisable()
        {
            SaveAsset();
        }
#endregion

        /// <summary>
        /// Called by the graph when a node is selected in it
        /// </summary>
        /// <param name="node"></param>
        internal void OnNodeSelected(NodeBase node)
        {
            _inspector.Clear();
            DestroyImmediate(CustomEditor);

            CustomEditor = (TreeAssetEditor)UnityEditor.Editor.CreateEditor(_asset, typeof(TreeAssetEditor));
            CustomEditor.SelectedId = node == null ? 0 : node.Id;
            _inspector.Add(CustomEditor.CreateInspectorGUI());
            _inspector.onGUIHandler = () => {
                if (CustomEditor.target)
                    CustomEditor.OnInspectorGUI();
            };
        }

        /// <summary>
        /// Creates the top toolbar
        /// </summary>
        private void CreateToolbar()
        {   
            var toolBar = new Toolbar();
   
            var localVariableToggle = new ToolbarToggle() { label = "Local Variables" };            
            _localVariables = new Blackboard(_graph) 
            { 
                title = "Local variables",
                subTitle = ""
            };
            localVariableToggle.RegisterValueChangedCallback(ShowLocalVariables);

            _localVariables.SetPosition(new Rect(position.xMax - 210, 30, 200, 300));
            _localVariables.style.visibility = Visibility.Hidden;
            _localVariables.addItemRequested = AddLocalVariable;

            var removeButton = new Button(() => RemoveLocalVariable(_localVariables)) { text = "-" };
            removeButton.style.fontSize = 20;
            removeButton.style.alignSelf = Align.Center;
            removeButton.style.backgroundColor = Color.clear;
            _localVariables.Q<VisualElement>("header").Add(removeButton);

            var assetField = new ObjectField("Asset")
            { objectType = typeof(TreeAsset), value = _asset };
            assetField.RegisterValueChangedCallback(OnAssetChanged);

            var validateButton = new Button(() => {
                _graph.TrimAndBuildAssetTree();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }) { text = "Build" };

            _graph.Add(_localVariables);
            toolBar.Add(localVariableToggle);
            toolBar.Add(assetField);
            toolBar.Add(validateButton);

            rootVisualElement.Insert(0,toolBar);
            void ShowLocalVariables(ChangeEvent<bool> evt)
            {
                _localVariables.style.visibility = evt.newValue ? Visibility.Visible : Visibility.Hidden;
                _localVariables.SetPosition(new Rect(position.xMax - 210, 30, 200, 300));
            }

            void OnAssetChanged(ChangeEvent<UnityEngine.Object> evt)
            {
                //rootVisualElement.Unbind();
                LoadAsset(evt.newValue as TreeAsset);
            }
        }

        /// <summary>
        /// Loads a new tree asset
        /// </summary>
        /// <param name="asset"></param>
        private void LoadAsset(TreeAsset asset)
        {   
            // Save old
            SaveAsset();

            // Populate graph
            _graph.Populate(asset);

            // Populate local variables
            _localVariables.contentContainer.Clear();
            if (asset != null)
            {
                foreach (var var in asset.LocalVariables)
                {
                    var textField = new TextField() { value = var };
                    textField.RegisterValueChangedCallback((evt) =>
                    {
                        var index = _localVariables.contentContainer.IndexOf((evt.target as VisualElement));
                        _asset.LocalVariables[index] = evt.newValue;
                    });
                    _localVariables.contentContainer.Add(textField);
                }
                _assetSo = new SerializedObject(asset);
                rootVisualElement.Bind(_assetSo);
            }
            else
            {
                DestroyImmediate(CustomEditor);
                rootVisualElement.Unbind();
            }
            
            _asset = asset;            
        }

        /// <summary>
        /// Adds a new local variable definition to the tree
        /// </summary>
        /// <param name="bb"></param>
        private void AddLocalVariable(Blackboard bb)
        {
            if(_asset != null)
            {
                var textField = new TextField();
                _asset.LocalVariables.Add("");
                textField.RegisterValueChangedCallback((evt) => 
                {
                    var index = bb.contentContainer.IndexOf((evt.target as VisualElement));
                    _asset.LocalVariables[index] = evt.newValue;
                });
                bb.contentContainer.Add(textField);
            }
        }

        /// <summary>
        /// Removes a local variable definition from the tree
        /// </summary>
        /// <param name="bb"></param>
        private void RemoveLocalVariable(Blackboard bb)
        {
            if(_asset != null && _asset.LocalVariables.Count != 0)
            {
                _asset.LocalVariables.RemoveAt(_asset.LocalVariables.Count - 1);
                bb.RemoveAt(_asset.LocalVariables.Count - 1);
            }
        }

        /// <summary>
        /// Creates the graph window
        /// </summary>
        private void CreateGraph()
        {
            _graph = new TreeGraphView(this);
            _graph.style.height = position.height;
            _graph.style.width = position.width * 0.8f;
            _twoPaneSplitView.Add(_graph);

            //var nodeBlackboard = new Blackboard(_graph);
            //nodeBlackboard.title = "Node Properties";
            //nodeBlackboard.subTitle = "";
            //nodeBlackboard.SetPosition(new Rect(10, 30, 500, 300));
            //nodeBlackboard.Q<Button>("addButton").RemoveFromHierarchy();
            var branchTemplates = new Blackboard(_graph)
            {
                title = "Branch Templates",
                subTitle = ""
            };
            branchTemplates.SetPosition(new Rect(10, 350, 200, 300));
            branchTemplates.addItemRequested = (bb) =>
            {
                BranchPopup.Open(_branchCont, (s) => AddBranch(s, bb));
            };

            //_graph.NodePropertyBoard = nodeBlackboard;
            _graph.Add(branchTemplates);
            //_graph.Add(nodeBlackboard);
        }

        /// <summary>
        /// Adds a new branch to the branch container. The name must be valid before calling this
        /// </summary>
        /// <param name="name"></param>
        /// <param name="board"></param>
        private void AddBranch(string name, Blackboard board)
        {   
            var validNodes = new List<NodeBase>();
            
            int validCount = 0;
            foreach(var node in _graph.selection)
            {
                if(node is BTNode visNode && !visNode.IsRoot)
                {
                    // Discard root if possible
                    var currentPos = visNode.Base.Position;
                    var parentId = currentPos.inputId == -1 ? 0 : currentPos.inputId;

                    // Copy position
                    var pos = new Position() { inputId = parentId, outputIds = new() };
                    foreach(var id in currentPos.outputIds)
                    {
                        pos.outputIds.Add(id);
                    }

                    // Create a default instance copy of the used type
                    var instance = NodeFactory.CreateNew(visNode.Base.GetType());
                    instance.UpdatePositionData(pos);
                    validNodes.Add(instance);
                    validCount++;
                }
            }
            if (validCount > 1)
            {
                var branch = new Branch(name, validNodes);
                // **TEMP**
                var branchField = new Label(name);

                board.contentContainer.Add(branchField);
                _branchCont.Templates.Add(name, branch);
            }
            else
                Debug.LogWarning("Selection must contain atleast 2 valid nodes excluding root");
        }

        /// <summary>
        /// Loads branch templates from disc
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        private void LoadTemplates()
        {   
            // Should find it from packages as well
            var cont = Resources.Load<BranchContainer>("Branches");
            if(cont == null)
            {
                cont = CreateInstance<BranchContainer>();

                // Non UPM location
                var subPath = "NPC AI Engine/Editor/Resources";
                var path = string.Concat(Application.dataPath, "/", subPath);
                if (Directory.Exists(path))
                    AssetDatabase.CreateAsset(cont, string.Concat("Assets/", subPath, "/Branches.asset"));
                else
                    throw new System.Exception("Failed to load branch templates. Package reinstallation required");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            } 
            _branchCont = cont;
        }

        /// <summary>
        /// Saves the asset
        /// </summary>
        private void SaveAsset()
        {
            if (_asset != null)
            {   
                EditorUtility.SetDirty(_asset);
                if(_graph != null)
                    _graph.TrimAndBuildAssetTree();
                AssetDatabase.SaveAssetIfDirty(_asset);
                AssetDatabase.Refresh();
            }
        }
    }
}

