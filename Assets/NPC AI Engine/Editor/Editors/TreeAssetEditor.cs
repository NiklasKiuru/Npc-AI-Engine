using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.Progress;
using UnityEngine.UIElements;
using System.Reflection;

namespace Aikom.AIEngine.Editor
{
    [CustomEditor(typeof(TreeAsset))]
    public class TreeAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _localVariables;
        private SerializedProperty _nodes;
        private SerializedProperty _root;

        /// <summary>
        /// Selected node id in the custom inspector
        /// </summary>
        internal int SelectedId { get; set; } = 0;

        private void OnEnable()
        {
            _localVariables = serializedObject.FindProperty("_localVariableNames");
            _nodes = serializedObject.FindProperty("_nodes");
            _root = serializedObject.FindProperty("_root");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var tree = (TreeAsset)target;
            if(SelectedId == 0 )
                goto Update;

            GUILayout.Space(25);
            var node = tree.GetNode(SelectedId);
            SerializedProperty prop;
            if (node is Root)
                prop = _root;
            else
                prop = _nodes.GetArrayElementAtIndex(tree.IndexOf(SelectedId));
            var attr = node.GetType().GetCustomAttribute<EditorNodeAttribute>();
            if(attr != null)
            {   
                EditorGUI.indentLevel++;
                GUI.enabled = false;
                EditorGUILayout.TextArea(attr.Description, new GUIStyle(EditorStyles.textArea) { wordWrap = true });
                // Draw base scriptfield
                MonoScript script = null;
                EditorGUILayout.ObjectField(script, typeof(TreeAsset), false);
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

           

            // Draw the selected node
            EditorGUILayout.PropertyField(prop, new GUIContent(NodeDescriptor.GetDefaultPrettyName(node)), true);
            prop.isExpanded = true;

            DrawUILine(new Color(0.157f, 0.157f, 0.157f, 1.000f), 1);

            // Draw misc editor properties
            EditorGUILayout.Separator();
            EditorGUI.indentLevel++;
            
            var desc = prop.FindPropertyRelative("_desc");
            var comment = desc.FindPropertyRelative("_userComment");
            var name = desc.FindPropertyRelative("_name");
            
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.LabelField("User comment:");
            var text = EditorGUILayout.TextArea(comment.stringValue, new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                fixedHeight = 50,
            });
            comment.stringValue = text;

            EditorGUI.indentLevel--;

        Update:
                serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var newRoot = new VisualElement();
            var header = new Label("Node Properties") 
            { 
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 14,
                }
            };
            newRoot.Add(header);
            return newRoot;
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }

}
