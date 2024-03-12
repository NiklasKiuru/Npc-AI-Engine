using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Aikom.AIEngine.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private TreeGraphView _graphView;
        private Texture2D _indentationIcon;

        public event Action<bool> OnWindowClosed;

        public void Configure(EditorWindow window, TreeGraphView graphView)
        {
            _window = window;
            _graphView = graphView;

            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0)
            };
            AddEntries<DecoratorNode>("Decorator");
            AddEntries<CompositeNode>("Composite");
            AddEntries<LeafNode>("Leaf");

            void AddEntries<T>(string category) where T : NodeBase
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent(category), 1));
                foreach (var type in TypeCache.GetTypesDerivedFrom<T>())
                {
                    if (type.IsAbstract)
                        continue;
                    var instance = CreateInstance(type);
                    var name = type.Name.Replace("Node", "");
                    tree.Add(new SearchTreeEntry(new GUIContent(name, _indentationIcon)) { level = 2, userData = instance });
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            var data = SearchTreeEntry.GetUserDataNode<NodeBase>();
            if (data != null)
            {
                _graphView.CreateNewDisplayNode(data, graphMousePosition);
                OnWindowClosed?.Invoke(true);
                return true;
            }
            OnWindowClosed?.Invoke(true);
            return false;
        }
    }

    internal static class TreeEntryExtensions
    {
        public static T GetUserDataNode<T>(this SearchTreeEntry entry) where T : NodeBase
        {
            return (T)entry.userData;
        }
    }
}
