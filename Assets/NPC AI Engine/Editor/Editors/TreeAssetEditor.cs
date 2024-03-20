using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.Progress;

namespace Aikom.AIEngine.Editor
{
    //[CustomEditor(typeof(TreeAsset))]
    public class TreeAssetEditor : UnityEditor.Editor
    {
        //private SerializedProperty _localVariables;
        //private SerializedProperty _nodes;
        //private SerializedProperty _root;

        //private void OnEnable()
        //{
        //    _localVariables = serializedObject.FindProperty("_localVariableNames");
        //    _nodes = serializedObject.FindProperty("_nodes");
        //    _root = serializedObject.FindProperty("_root");
        //}

        //public override void OnInspectorGUI()
        //{
        //    serializedObject.Update();

        //    DrawArray(_localVariables, "Local Variables");
        //    EditorGUILayout.PropertyField(_root);
        //    DrawArray(_nodes, "Nodes");

        //    serializedObject.ApplyModifiedProperties();

        //    void DrawArray(SerializedProperty prop, string name)
        //    {
        //        prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, name);
        //        if (prop.isExpanded)
        //        {
        //            EditorGUI.indentLevel++;
        //            if (prop.arraySize < 0)
        //                prop.arraySize = 0;
        //            // draw item fields
        //            for (var i = 0; i < prop.arraySize; i++)
        //            {
        //                var item = prop.GetArrayElementAtIndex(i);
        //                EditorGUILayout.PropertyField(item);
        //            }

        //            EditorGUI.indentLevel--;
        //        }
        //    }
        //}
    }

}
