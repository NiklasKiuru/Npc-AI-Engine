using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using System;

namespace Aikom.AIEngine.Editor
{   
    /// <summary>
    /// Editor for tree assets
    /// </summary>
    [CustomEditor(typeof(TreeAsset))]
    public class TreeAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _nodes;
        private SerializedProperty _root;

        /// <summary>
        /// Selected node id in the custom inspector
        /// </summary>
        internal int SelectedId { get; set; } = 0;

        private void OnEnable()
        {
            _nodes = serializedObject.FindProperty("_nodes");
            _root = serializedObject.FindProperty("_root");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

                // Draw base scriptfield. For cs files in packages this should return null so we ignore it
                MonoScript script = FindScriptAsset(node.GetType());
                if(script != null) 
                    EditorGUILayout.ObjectField(script, node.GetType(), false);
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Draws a simple line in GUI
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// Finds the script asset of this type. The filename of the type must match the type name exactly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private MonoScript FindScriptAsset(Type type)
        {   
            var name = type.Name + ".cs";
            foreach(var path in AssetDatabase.GetAllAssetPaths())
            {
                if(path.EndsWith(name))
                    return AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            }
            return null;
        }
    }
}
