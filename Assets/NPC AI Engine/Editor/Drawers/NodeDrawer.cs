using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Aikom.AIEngine.Editor
{
    [CustomPropertyDrawer(typeof(EditorNodeAttribute))]
    public class NodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = property.managedReferenceValue.GetType().GetCustomAttribute<EditorNodeAttribute>();
            EditorGUI.LabelField(position, new GUIContent(attr.Description));
            GUILayout.Space(20);
            base.OnGUI(position, property, label);
        }
    }

}
