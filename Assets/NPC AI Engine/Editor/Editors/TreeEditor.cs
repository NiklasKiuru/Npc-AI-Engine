using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Graphs;

namespace Aikom.AIEngine.Editor
{
    [Serializable]
    public class TreeEditor : EditorWindow
    {
        [SerializeField] private BranchContainer _branchCont;
        [SerializeField] private TreeAsset _asset;

        private TreeGraphView _graph;
        private Blackboard _localVariables;
        private BehaviourTree _cachedRuntimeTree;

        public TreeAsset Asset { get { return _asset; } }   

        [MenuItem("Window/Behaviour Tree")]
        public static void Init()
        {
            var window = GetWindow<TreeEditor>();
            window.titleContent = new GUIContent("Behaviour Tree");
            window.Show();
        }

        private void CreateGUI()
        {
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
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {

            }
        }

        private void OnSelectionChange()
        {
            if (_cachedRuntimeTree != null)
                _cachedRuntimeTree.OnTickCallback -= _graph.OnRuntimeTick;
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

        private void CreateToolbar()
        {   
            var toolBar = new Toolbar();
            rootVisualElement.Add(toolBar);

            var localVariableToggle = new ToolbarToggle() { label = "Local Variables" };            
            _localVariables = new Blackboard(_graph) 
            { 
                title = "Local variables",
                subTitle = ""
            };

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
            localVariableToggle.RegisterValueChangedCallback(ShowLocalVariables);

            void ShowLocalVariables(ChangeEvent<bool> evt)
            {
                _localVariables.style.visibility = evt.newValue ? Visibility.Visible : Visibility.Hidden;
            }

            void OnAssetChanged(ChangeEvent<UnityEngine.Object> evt)
            {
                LoadAsset(evt.newValue as TreeAsset);
            }
        }

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
            }

            _asset = asset;            
        }

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

        private void RemoveLocalVariable(Blackboard bb)
        {
            if(_asset != null && _asset.LocalVariables.Count != 0)
            {
                _asset.LocalVariables.RemoveAt(_asset.LocalVariables.Count - 1);
                bb.RemoveAt(_asset.LocalVariables.Count - 1);
            }
        }

        private void CreateGraph()
        {
            _graph = new TreeGraphView(this);
            _graph.StretchToParentSize();
            rootVisualElement.Add(_graph);

            var nodeBlackboard = new Blackboard(_graph);
            nodeBlackboard.title = "Node Properties";
            nodeBlackboard.subTitle = "";
            nodeBlackboard.SetPosition(new Rect(10, 30, 500, 300));
            nodeBlackboard.Q<Button>("addButton").RemoveFromHierarchy();
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

            _graph.NodePropertyBoard = nodeBlackboard;
            _graph.Add(branchTemplates);
            _graph.Add(nodeBlackboard);
        }

        // The name has to be valid before calling this
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

        private void OnDisable()
        {
            SaveAsset();
        }
    }
}
